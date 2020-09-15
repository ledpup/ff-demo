using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDS.CourseAdminMastering.WebApi
{
    /// <summary>
    /// ProgramFeatureFlagConstants
    /// </summary>
    public class ProgramFeatureFlagConstants
    {
        public const string EnableBrms = "CourseAdmin.Program.Settings.BRMS";
        public const string EnableMastering = "CourseAdmin.Program.Settings.Mastering";
        public const string EnableContinueValidationOnBehaviourRulesError = "CourseAdmin.Program.Settings.ContinueValidationOnBehaviourRulesError";
    }

    /// <summary>
    /// SubjectFeatureFlagConstants
    /// </summary>
    public class SubjectFeatureFlagConstants
    {
        public const string EnableBrms = "CourseAdmin.Subject.Settings.BRMS";
        public const string EnableMastering = "CourseAdmin.Subject.Settings.Mastering";
        public const string EnableContinueValidationOnBehaviourRulesError = "CourseAdmin.Subject.Settings.ContinueValidationOnBehaviourRulesError";
    }
}
