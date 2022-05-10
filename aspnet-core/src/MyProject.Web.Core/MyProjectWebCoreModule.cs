// This file is not generated, but this comment is necessary to exclude it from StyleCop analysis 
// <auto-generated/> 
using System;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.SignalR;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.Configuration;
using MyProject.Authentication.JwtBearer;
using MyProject.Configuration;
using MyProject.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.IO;
using Abp.IO;

namespace MyProject
{
    [DependsOn(
         typeof(MyProjectApplicationModule),
         typeof(MyProjectEntityFrameworkModule),
         typeof(AbpAspNetCoreModule)
        , typeof(AbpAspNetCoreSignalRModule)
     )]
    public class MyProjectWebCoreModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public MyProjectWebCoreModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                MyProjectConsts.ConnectionStringName
            );

            // Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Modules.AbpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(MyProjectApplicationModule).GetAssembly()
                 );

            ConfigureTokenAuth();
        }

        private void ConfigureTokenAuth()
        {
            IocManager.Register<TokenAuthConfiguration>();
            var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();

            tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
            tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
            tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            tokenAuthConfig.Expiration = TimeSpan.FromDays(1);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MyProjectWebCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(MyProjectWebCoreModule).Assembly);
            SetAppFolders();
        }

        private void SetAppFolders()
        {
            var appFolders = IocManager.Resolve<AppFolders>();
            // file export
            appFolders.TempFileDownloadFolder = Path.Combine(_env.WebRootPath, $"Temp{Path.DirectorySeparatorChar}Downloads");
            // Chứa file upload phần Demo
            appFolders.DemoUploadFolder = Path.Combine(_env.WebRootPath, $"Upload{Path.DirectorySeparatorChar}Demo");

            // Folder chứ file mẫu cho phần import Demo
            appFolders.DemoFileDownloadFolder = Path.Combine(_env.WebRootPath, $"Temp{Path.DirectorySeparatorChar}Downloads{Path.DirectorySeparatorChar}Imports{Path.DirectorySeparatorChar}Demo");

            try
            {
                DirectoryHelper.CreateIfNotExists(appFolders.TempFileDownloadFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.DemoUploadFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.DemoFileDownloadFolder);
            }
            catch { }
        }
    }
}
