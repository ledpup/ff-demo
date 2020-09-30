using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using System;
using System.Threading.Tasks;

namespace VDS.CourseAdminMastering.WebApi.Controllers
{

    [ApiController]
    public class ProgramController : BaseController
    {
        private ILogger<ProgramController> _logger;
        private readonly IFeatureManager _featureManager;
        IDisposable _simpleDisposableClass;
        private bool _enableBrms;
        private bool _enableMastering;
        private bool _continueValidationOnBehaviourRulesError;

        public ProgramController(ILogger<ProgramController> logger, IFeatureManager featureManager, IDisposable simpleDisposableClass)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
            _simpleDisposableClass = simpleDisposableClass;
        }

        [HttpPost("Program/Master/Validate")]
        public async Task<ActionResult<string>> Validate([FromBody] ServiceBusMessage message)
        {
            await SetFeatureFlags();
            return "Done";
        }

        private async Task SetFeatureFlags()
        {
            _continueValidationOnBehaviourRulesError = true;
            _enableBrms = true;
            _enableMastering = true;
            if (WebApi.AppConfigExists)
            {
                _continueValidationOnBehaviourRulesError = await _featureManager.IsEnabledAsync(ProgramFeatureFlagConstants.EnableContinueValidationOnBehaviourRulesError);
                _enableBrms = await _featureManager.IsEnabledAsync(ProgramFeatureFlagConstants.EnableBrms);
                _enableMastering = await _featureManager.IsEnabledAsync(ProgramFeatureFlagConstants.EnableMastering);
            }

            _logger.LogInformation($"AppConfig { (WebApi.AppConfigExists ? "found" : "not found") }. ContinueValidationOnBehaviourRulesError = {_continueValidationOnBehaviourRulesError}; EnableBrms = {_enableBrms}; EnableMastering = {_enableMastering}.");
        }

        public class ServiceBusMessage
        {
        }
    }
}
