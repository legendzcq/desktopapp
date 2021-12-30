# 下载课堂VNext

## 技术选型

### 外观

如果开发人员**不具备**复现设计师设计的能力，最好选用一款公共设计语言的开源控件库，优点在于这种控件库不仅有现代化的视觉体验，还能大大缩减设计和开发进程，通过自定义微调也能够满足公司的品牌要求。

针对两大设计语言Material Design及Fluent Design，WPF均有相关的开源控件库，Fluent Design是Window 10的设计语言，目前不支持Window 7，所有可供选择的只有[Material Design Toolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)，这一点是区别于Web前端繁荣的社区生态的。

### 架构

MVVM架构是所有带UI应用程序（不考虑服务端渲染的场景）的通用选择，不管是桌面应用还是SPA应用，都会涉及到数据绑定及组件化开发，WPF对MVVM及组件化开发的支持非常完备，支持WPF的几大主流MVVM框架均做过比较深入的探索，[Prism](https://github.com/PrismLibrary/Prism)是**最佳的选择**，Prism的核心概念Region非常强大，严格按照最佳实践可轻易获得一个松散耦合的架构，而且很容易集成第三方库（集成的好需要对Region Adapter有深刻的理解），它对WPF的原生命令进行了包装，让防御式的命令处理变得更加简单。

### 容器

Prism支持多种主流容器，但是很遗憾，Prism的诸多功能需要用到命名容器（即通过字符串从容器中取出对象），[MSDI](https://github.com/dotnet/runtime/tree/main/src/libraries/Microsoft.Extensions.DependencyInjection)恰好不支持，但是可通过[其他方式](https://prismplugins.com/containers/microsoft-extensions-dependencyinjection/)集成MSDI。

**使用MSDI非常重要**，在.Net Core的世界，很多官方库都对MSDI有着完备的支持，比如持久化、配置、日志等等。

### 持久化

持久化部分推荐使用[Entity Framework Core](https://github.com/dotnet/efcore)，它作为Data Mapping ORM的典型代表，可以实现POCO（不依赖除了框架自带类型的其他类型）模型，建议使用充血模型而非贫血模型，充分表达业务语义，建议使用Repository模式来进一步隔离数据的处理。使用EF Core你将获得强类型的诸多好处，大大减少数据处理相关的代码量。

### 外部接口

可尝试使用依赖倒置原则，让数据模型所处的层持有外部调用的接口，接口的实现则放在基础设施层，这实际上就是端口适配器模式的实现。对Web API的调用推荐使用[Refit](https://github.com/reactiveui/refit)，Refit可自动帮你构建HttpClient，对数据进行序列化及反序列化处理，而且还能获得强类型的诸多好处，大大减少Web API调用相关代码量。

### 表单验证

没有很好的基础设施提供支持的话，客户端表单验证是复杂而枯燥的，而且很难同ViewModel结合起来，ValidationContext可把基于注解的表单验证同INotifyChanged接口相结合提供响应式的表单验证体验，基于注解还提供了原生的i18N支持（也可以选择不用）。

代码示例：

```csharp
public abstract class ValidateableBase : BindableBase, INotifyDataErrorInfo
    {
        private readonly ErrorsContainer<string> _errorsContainer;

        protected ValidateableBase()
        {
            _errorsContainer = new ErrorsContainer<string>(RaiseErrorsChanged);
            ErrorsChanged += OnErrorsChanged;
        }

        private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(HasErrors));
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public bool HasErrors => _errorsContainer.HasErrors;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsContainer.GetErrors(propertyName);
        }

        protected virtual void ValidateProperty(object value, [CallerMemberName]string propertyName = null)
        {
            var context = new ValidationContext(this)
            {
                MemberName = propertyName
            };
            var validationErrors = new List<ValidationResult>();

            if (!Validator.TryValidateProperty(value, context, validationErrors))
            {
                IEnumerable<string> errors = validationErrors.Select(x => x.ErrorMessage);
                _errorsContainer.SetErrors(propertyName, errors);
            }
            else
            {
                _errorsContainer.ClearErrors(propertyName);
            }
        }

        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            ValidateProperty(value, propertyName);
            return base.SetProperty(ref storage, value, propertyName);
        }

        protected override bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            ValidateProperty(value, propertyName);
            return base.SetProperty(ref storage, value, onChanged, propertyName);
        }

        protected static bool IsDirty(string value)
        {
            return value != null;
        }
    }
```

调用示例：
```csharp
public class LoginWindowViewModel : ValidateableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISignInManager _signInManager;

        public LoginWindowViewModel(
            IEventAggregator eventAggregator,
            ISignInManager signInManager,
            IEventStore eventStore)
        {
            _eventAggregator = eventAggregator;
            _signInManager = signInManager;
            _eventStore = eventStore;
        }

        private string _username;

        [Required(ErrorMessage = "用户名不能为空")]
        public string Username
        {
            get => _username;
            set
            {
                Visibility = Visibility.Collapsed;
                SetProperty(ref _username, value);
            }
        }

        private string _password;

        [Required(ErrorMessage = "密码不能为空")]
        public string Password
        {
            get => _password;
            set
            {
                Visibility = Visibility.Collapsed;
                SetProperty(ref _password, value);
            }
        }

        private Visibility _visibility = Visibility.Collapsed;

        public Visibility Visibility
        {
            get => _visibility;
            set => SetProperty(ref _visibility, value);
        }

        private ObservableCollection<string> _signInHistory;

        public ObservableCollection<string> SignInHistory
        {
            get => _signInHistory;
            private set => SetProperty(ref _signInHistory, value);
        }

        private string _signInResult;

        public string SignInResult
        {
            get => _signInResult;
            set => SetProperty(ref _signInResult, value);
        }

        private DelegateCommand _loadedCommand;

        public DelegateCommand LoadedCommand =>
            _loadedCommand ?? (_loadedCommand = new DelegateCommand(ExecuteLoadedCommand));

        private void ExecuteLoadedCommand()
        {
            SignInHistory = new ObservableCollection<string>(_signInManager.SignInHistory.History);
        }

        private DelegateCommand _loginCommand;

        public DelegateCommand LoginCommand =>
            _loginCommand ?? (_loginCommand = new DelegateCommand(ExecuteLoginCommand, CanExecuteLoginCommand))
            .ObservesProperty(() => HasErrors)
            .ObservesProperty(() => Username)
            .ObservesProperty(() => Password);

        private bool CanExecuteLoginCommand()
        {
            return !HasErrors && IsDirty(Username) && IsDirty(Password);
        }

        private async void ExecuteLoginCommand()
        {
            SignInResult result = await _signInManager.SignInAsync(Username, Password).ConfigureAwait(false);
            SignInResult = result.ToString();
            if (result.Succeeded)
            {
                _eventStore.Insert(PersistentEvent.CreateOperatorLoginEvent(_signInManager.CurrentUser.Username));
                _signInManager.SignInHistory.InsertIfNotExist(Username);
                _eventAggregator.GetEvent<LoginSucceedEvent>().Publish();
            }
            else
            {
                Visibility = Visibility.Visible;
            }
        }
    }
```

## 模块治理

建议按照业务和基础设施两个维度实施模块划分，其中业务模块是依赖于基础设施模块的，这样做的好处是可灵活装配模块来组装成一个完整的应用程序，便于代码复用及简化依赖关系。

业务模块的识别可根据次级导航来进行，比如课程模块、课程讲义模块、题库模块等，可根据需要对业务模块进行进一步的拆分。

**业务模块可视为一个库项目，里面有完整的MVVM架构，可以类比微服务**。

独立出来的基础设施模块应该是通用的，不通用的基础设施内容可融合进各个业务模块。

## 最佳实践

**应当减少对Window对象的滥用**，Window对象的初始化成本比较高，多窗体的遮罩层也不好实现，管理成本更高，应减少对Window对象的滥用，这一点可通过Prism提供的占位符来轻松解决。

严格区别交互和业务之间的关系，业务代码应在ViewModel中处理，交互相关的内容应在View中处理。

ViewModel中涉及IO的操作应当以异步的方式运行，提高程序性能，而不应占用UI线程的资源。

