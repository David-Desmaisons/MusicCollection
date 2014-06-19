using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Deployment.Application;
using System.Reflection;

namespace MusicCollection.Infra
{
    public static class VersionInfo
    {
        public static Version GetVersionInfo()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }

            return Assembly.GetExecutingAssembly().GetName().Version;
        }
   
    }
}
