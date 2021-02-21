using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RuntimeSetup
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new RuntimeSetupStack(app, "RuntimeSetupStack");
            app.Synth();
        }
    }
}
