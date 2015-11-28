using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;

using mvc4gw7.Models; 

namespace mvc4gw7
{
  public static class Bootstrapper
  {
    public static IUnityContainer Initialise()
    {
      var container = BuildUnityContainer();

      DependencyResolver.SetResolver(new UnityDependencyResolver(container));

      return container;
    }

    private static IUnityContainer BuildUnityContainer()
    {
      var container = new UnityContainer();

      // register all your components with the container here
      // it is NOT necessary to register your controllers

      // e.g. container.RegisterType<ITestService, TestService>();]
      //container.RegisterType<IAppMediaRepository, GrowndworkAppMediaRepository>();
      container.RegisterType<IAppMediaRepository, LINQAppMediaRepository>();
      //container.RegisterType<IAppMediaRepository, XMLAppMediaRepository>();
      //container.RegisterType<IAppMediaRepository, PostgreSQLAppMediaRepository>();              

      RegisterTypes(container);


      return container;
    }

    public static void RegisterTypes(IUnityContainer container)
    {
    
    }
  }
}
