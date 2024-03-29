using System;
using Assets.UnityTestTools.IntegrationTestsFramework.TestRunner;
using UnityEngine;

namespace Assets.UnityTestTools.Examples.IntegrationTestsFrameworkExamples
{
    [IntegrationTest.DynamicTestAttribute("ExampleIntegrationTests")]
// [IntegrationTest.Ignore]
    [IntegrationTest.ExpectExceptions(false, typeof(ArgumentException))]
    [IntegrationTest.SucceedWithAssertions]
    [IntegrationTest.TimeoutAttribute(1)]
    [IntegrationTest.ExcludePlatformAttribute(RuntimePlatform.Android, RuntimePlatform.LinuxPlayer)]
    public class DynamicIntegrationTest : MonoBehaviour
    {
        public void Start()
        {
            IntegrationTest.Pass(gameObject);
        }
    }
}
