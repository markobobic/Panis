// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Panis.DependencyResolution {
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.Owin.Security;
    using Panis.Interfaces;
    using Panis.Models;
    using Panis.Services;
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;
    using System.Data.Entity;
    using System.Web;

    public class DefaultRegistry : Registry {
        #region Constructors and Destructors

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
					scan.With(new ControllerConvention());
                });
            //For<IExample>().Use<Example>();
            For<DbContext>().Use<ApplicationDbContext>().Transient();
            For<IAbsence>().Use<AbsenceRepo>().Transient();
            For<ITeamLead>().Use<TeamLeadRepo>().Transient();
            For<IUserRepo>().Use<UserRepo>().Transient();
            For<IEmployee>().Use<EmployeeRepo>().Transient();
            For<IEmployeeEnrollments>().Use<EmployeeEnrollmentRepo>().Transient();
            For<INotification>().Use<NotificationRepo>().Transient();
            For<IUserStore<ApplicationUser>>().Use<UserStore<ApplicationUser>>();
            For<DbContext>().Use(() => new ApplicationDbContext());
            For<IAuthenticationManager>().Use(() => HttpContext.Current.GetOwinContext().Authentication);
            For<IRealization>().Use<RealizationRepo>().Transient();
            For<IProject>().Use<ProjectRepo>().Transient();

        }

        #endregion
    }
}