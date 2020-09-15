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
        private readonly IFeatureManagerSnapshot _featureManagerSnapshot;
        private bool _enableBrms;
        private bool _enableMastering;
        private bool _continueValidationOnBehaviourRulesError;

        public ProgramController(ILogger<ProgramController> logger, IFeatureManagerSnapshot featureManagerSnapshot)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _featureManagerSnapshot = featureManagerSnapshot ?? throw new ArgumentNullException(nameof(featureManagerSnapshot));
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
                _continueValidationOnBehaviourRulesError = await _featureManagerSnapshot.IsEnabledAsync(ProgramFeatureFlagConstants.EnableContinueValidationOnBehaviourRulesError);
                _enableBrms = await _featureManagerSnapshot.IsEnabledAsync(ProgramFeatureFlagConstants.EnableBrms);
                _enableMastering = await _featureManagerSnapshot.IsEnabledAsync(ProgramFeatureFlagConstants.EnableMastering);
            }

            _logger.LogInformation($"AppConfig { (WebApi.AppConfigExists ? "found" : "not found") }. ContinueValidationOnBehaviourRulesError = {_continueValidationOnBehaviourRulesError}; EnableBrms = {_enableBrms}; EnableMastering = {_enableMastering}.");
        }

        public class ServiceBusMessage
        {
        }
    }
}
