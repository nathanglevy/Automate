using System;
using UnityEngine;

namespace Assets.UnityTestTools.Examples.IntegrationTestsFrameworkExamples
{
    public class ThrowCustomException : MonoBehaviour
    {
        public void Start()
        {
            throw new CustomException();
        }

        private class CustomException : Exception
        {
        }
    }
}
