# .Net Core 小知識
<br><br>

### Filter
___
- Filter 的作用是在 Action 執行前或執行後做一些加工處理。
- 某種程度來看，會跟 Middleware 很像，但執行的順序略有不同，用對 Filter 不僅可以減少程式碼，還可以減省執行效率。
- 共有五種 Filters
    1. Authorization Filter
        - Authorization 是五種 Filter 中優先序最高的，通常用於驗證 Request 合不合法，不合法後面就直接跳過
    1. Resource Filter
        - Resource 是第二優先，會在 Authorization 之後，Model Binding 之前執行。通常會是需要對 Model 加工處裡才用。
    1. Action Filter
        - 最容易使用的 Filter，封包進出都會經過它，使用上沒什麼需要特別注意的。跟 Resource Filter 很類似，但並不會經過 Model Binding。
    1. Exception Filter
        - 異常處理的 Exception。
    1. Result Filter
        - 當 Action 完成後，最終會經過的 Filter。
- 示意圖

    ![GitHub Logo](/img/Filter.png)


<br><br><br><br><br>

# Episode 4 的重點

1. IAuthorizationService (常用,很重要)
1. AuthorizeFilter
1. OperationAuthorizationRequirement (知道就好,很少被使用)
1. AuthorizationPolicyProvider (常用,很重要)



<br><br>

### 1. IAuthorizationService
___
* 使用時機
    - 當你 Authorize 得需求, 需要在一個 Action 內部因為 Authorize 的結果不同而有所不同時
* 使用方法:
    - In Controller
    1. 用以知的 Policy
        ```c#
        public async Task<IActionResult> UseAuthorizationService1()
        {
            var authoResult = await _authorizationService.AuthorizeAsync(HttpContext.User, "Technology");
            if (authoResult.Succeeded)
            {
                _logger.LogWarning("The user has been authorized!");
            }
            else
            {
                _logger.LogWarning("The user has not been authorized!");
            }
            return RedirectToAction("Index");
        }
        ```
    1. 自製 Policy
        ```c#
        public async Task<IActionResult> UseAuthorizationService2()
        {
            var policyBuilder = new AuthorizationPolicyBuilder();
            var policy = policyBuilder.RequireAuthenticatedUser().RequireClaim("age").Build();

            var authoResult = await _authorizationService.AuthorizeAsync(HttpContext.User, policy);
            if (authoResult.Succeeded)
            {
                _logger.LogWarning("The user has been authorized!");
            }
            else
            {
                _logger.LogWarning("The user has not been authorized!");
            }
            return RedirectToAction("Index");
        }
        ```
    - 在 Razor Page 內
        ```c#
        @using Microsoft.AspNetCore.Authorization;
        @inject IAuthorizationService _authorizationService;
        
        @if((await _authorizationService.AuthorizeAsync(User, "Claim.Level")).Succeeded)
        {
            <h1>The user has level claim</h1>
        }
        ```



<br><br>

### 2. AuthorizeFilter
___
* 使用 AuthorizeFilter & Authorization Middelware 最大的區別是, AuthorizeFilter 裡的 Policy 每一個 Action 都會執行, 但 Authorization Middleware 的 Policy 只有在 Action 明確標註 [Authorize] attribute 時會執行
* 若你很在意資訊安全的問題, 則可以把 Authorization Policy 放在 AuthorizeFilter 裡面, 然後利用 [AllowAnonymous] 放過幾個 Action
* 使用的方法:
    1. Register AuthorizeFilter
        ```c#
        services.AddControllersWithViews(options =>
        {
            var policyBuilder = new AuthorizationPolicyBuilder();
            var policy = policyBuilder.RequireAuthenticatedUser()
                                        .AddRequirements(new DepartmentRequirement("Technology"))
                                        .Build();

            options.Filters.Add(new AuthorizeFilter(policy));
        });
        ```
    1. 若有些 Action 不想要經過 AuthorizeFilter 提供的 Policy, 則可以放入以下 Action Attribute
        ```c#
        [AllowAnonymous]
        ```



<br><br>

### 3. OperationAuthorizationRequirement
___
1. 他是一種 Microsoft 自己內建好的 Authorization Requirement, 不常使用, 知道就好
1. 他原本是被發明用來處理一些 Operation Switch 的東西。 例如, Technology 部們的 operation 跟 Finance 部門的 operation 跟 supply 部門的 operation 要做一些區隔
1. 簡單範例
    1. 建立一個 Operation 常數類別
        ```c#
        public static class DepartmentOprations
        {
            public const string Technology = "Technology";
            public const string Finance = "Finance";
            public const string Operation = "Operation";
        }
        ```
    1. 建立一個 handle OperationAuthorizationRequirement 的 handler, 會依據 operation 的種類做判斷
        ```c#
        public class DepartmentAuthorizationHandler :
            AuthorizationHandler<OperationAuthorizationRequirement,int>
        {

            protected override Task HandleRequirementAsync(
                AuthorizationHandlerContext context, 
                OperationAuthorizationRequirement requirement,
                int level)
            {
                var departmentClaim = context.User.Claims.Where(x => x.Type == "department").SingleOrDefault();
                if (departmentClaim == null)
                    throw new ArgumentNullException(nameof(departmentClaim));
                var department = departmentClaim.Value;

                switch(department)
                {
                    case DepartmentOprations.Technology:
                        if (requirement.Name == DepartmentOprations.Technology)
                            context.Succeed(requirement);
                        break;
                    case DepartmentOprations.Finance:
                        if (requirement.Name == DepartmentOprations.Finance)
                            context.Succeed(requirement);
                        break;
                    case DepartmentOprations.Operation:
                        if (requirement.Name == DepartmentOprations.Operation)
                            context.Succeed(requirement);
                        break;
                    default:
                        break;
                }

                return Task.CompletedTask;
            }
        }    
        ```
    1. 接下來就可以使用了
        ```c#
        var policyBuilder = new AuthorizationPolicyBuilder();
        var policy = policyBuilder.RequireAuthenticatedUser()
                                .AddRequirements(GetOperationRequirement.Technology)
                                .Build();


        if((await _authorizationService.AuthorizeAsync(HttpContext.User, 1, policy)).Succeeded)
        {
        _logger.LogWarning("Do Something which only the admin in technology department can do");
        }
        else
        {
        _logger.LogWarning("Other than technology admin");
        }

        ```


<br><br>

### 4. AuthorizationPolicyProvider
___
* 使用目的 
    - 如果有要產生很多很重複很制式化的 Policy ,一個一個 Regsiter 會使得代碼變得很累贅, 這個時候就可以考慮使用 Authorizationprovider

* 使用範例
    - 這個範例主要是讓 Policy 以 {department}.{level} 的形式呈現. 其中, department 的部分, 是要拿來符合 Department Authorization Requirement 用的. 而 level 的部分, 則是拿來符合 Level Authorization Requirement 用的.
    - 步驟
    1. 寫一個 Policy 的 Factory, 從解讀文字 "{department}.{level}" 產生 Policy
        ```c#
        public static class CompanyPolicyFactory
        {
            public static AuthorizationPolicy Create(string policyName)
            {
                if(policyName != null)
                {
                    var split = policyName.Split('.');
                    var department = split[0];
                    if (!Int32.TryParse(split[1], out int level))
                        throw new ArgumentException("Level cannot be translated to integer.");

                    return new AuthorizationPolicyBuilder()
                                    .RequireAuthenticatedUser()
                                    .AddRequirements(new DepartmentRequirement(department))
                                    .AddRequirements(new LevelRequirement(level))
                                    .Build();
                }
                else
                {
                    return new AuthorizationPolicyBuilder()
                                    .RequireAuthenticatedUser()
                                    .Build();
                }
            }
        }
        ``` 
    1. 寫一個衍生自 DefaultAuthorizationPolicyProvider 的 AuthorizationPolicyProvider, 運用以上的 Policy Factory 產生對應的 Policy
        ```c#
        public class CompanyAuthorizationPolicyProvider
            : DefaultAuthorizationPolicyProvider
        {
            public CompanyAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
            {}

            // {department}.{level}
            public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
            {
                return Task.FromResult(CompanyPolicyFactory.Create(policyName));
            }
        }
        ```
    1. DI 剛剛寫好的 Policy Provider
        ```c#
        services.AddSingleton<IAuthorizationPolicyProvider, CompanyAuthorizationPolicyProvider>();
        ```
    1. 最後可以寫一個自定義的 Authorize Attribute 方便使用
        ```c#
        public class CompanyAuthorizeAttribute : AuthorizeAttribute
        {
            public CompanyAuthorizeAttribute(string department, int level)
            {
                Policy = $"{department}.{level}";
            }
        }
        ```
        ```c#
        [CompanyAuthorize("Technology",0)]
        ```