using StarFlintSaver.Windows.ViewModel;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace StarFlintSaver.Windows.Utils
{
    public sealed class ViewModelAnimationManager<TViewModelType>
        where TViewModelType : ViewModelBase
    {
        private readonly int _animationDurationInSeconds;
        private readonly int _animationHalfDurationInSeconds;
        private int _animationTimeCount = 0;

        private readonly object _animationStartTimeUpdateLockObject = new object();
        private object _animationAccessLockObject = null;

        private readonly Action<TViewModelType, bool> _startSetterAction;
        private readonly Action<TViewModelType, string> _messageSetterAction;

        private DateTime _animationStartTime;

        public ViewModelAnimationManager(
            int animationDurationInSeconds,
            Expression<Func<TViewModelType, bool>> startSetterExpression,
            Expression<Func<TViewModelType, string>> messageSetterExpression)
        {
            _animationDurationInSeconds = animationDurationInSeconds;
            // half time of the starting animation
            _animationHalfDurationInSeconds = animationDurationInSeconds / 2;

            _startSetterAction = MakeAssignAction(startSetterExpression);
            _messageSetterAction = MakeAssignAction(messageSetterExpression);
        }

        public void StartAnimation(TViewModelType viewModel, string message)
        {
            // message animation update
            _messageSetterAction?.Invoke(viewModel, message);

            var newObjectLock = new object();

            // lock access to the starting animation
            if (Interlocked.CompareExchange(ref _animationAccessLockObject, newObjectLock, null) == null)
            {
                _ = Task.Run(async () =>
                {
                    await ExecuteAnimation(viewModel);
                });
            }
            else
            {
                AddAnimationTime();
            }
        }

        private async Task ExecuteAnimation(TViewModelType viewModel)
        {
            // update the starting animation's start time
            lock (_animationStartTimeUpdateLockObject)
            {
                _animationStartTime = DateTime.Now;
            }

            // start animation: true
            _startSetterAction.Invoke(viewModel, true);

            // wait for starting animation ending
            var duration = TimeSpan.FromSeconds(_animationDurationInSeconds);
            if (Interlocked.Increment(ref _animationTimeCount) > 0)
            {
                await Task.Delay(duration);
            }

            // wait for next animation times
            while (Interlocked.Decrement(ref _animationTimeCount) > 0)
            {
                if (_animationHalfDurationInSeconds > 0)
                {
                    // add some time (half time of the starting animation)
                    var halfDuration = TimeSpan.FromSeconds(_animationHalfDurationInSeconds);
                    await Task.Delay(halfDuration);
                }
            }

            // end of all animations
            // start animation: false
            _startSetterAction?.Invoke(viewModel, false);
            // message animation update (empty)
            _messageSetterAction?.Invoke(viewModel, string.Empty);
            // unlock starting animation
            Interlocked.Exchange(ref _animationAccessLockObject, null);
        }

        private void AddAnimationTime()
        {
            // update next animation's start time and animation time count
            lock (_animationStartTimeUpdateLockObject)
            {
                var elapsedTime = DateTime.Now - _animationStartTime;
                // animation's period [0; _animationDurationInSeconds]
                if (elapsedTime < TimeSpan.FromSeconds(_animationDurationInSeconds))
                {
                    // add animation time
                    if (Interlocked.Increment(ref _animationTimeCount) > 0)
                    {
                        // new start time
                        _animationStartTime = _animationStartTime.AddSeconds(elapsedTime.TotalSeconds);
                    }
                }
            }
        }

        private static Action<TObjectType, TPropertyType> MakeAssignAction<TObjectType, TPropertyType>(Expression<Func<TObjectType, TPropertyType>> expression)
        {
            if (expression.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new ArgumentException("This should be a member getter", "expression");
            }

            // Input model
            var model = expression.Parameters[0];

            // Input value to set
            var value = Expression.Variable(typeof(TPropertyType), "newValue");

            // Member access
            var member = expression.Body;

            // We turn the access into an assignation to the input value
            var assignation = Expression.Assign(member, value);

            // We wrap the action into a lambda expression with parameters
            var assignLambda = Expression.Lambda<Action<TObjectType, TPropertyType>>(assignation, model, value);

            return assignLambda.Compile();
        }
    }
}
