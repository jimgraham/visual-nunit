﻿using System;
using System.IO;
using System.Collections;
using System.Reflection;
using NUnit.Core.Extensibility;
using NUnit.Core;

namespace VisualNunitLogic
{
    /// <summary>
    /// Class that builds a TestSuite from an assembly. This is modified version of AssemblyTestBuilder from Nunit core.
    /// </summary>
    public class TestBuilder
    {
        static Logger log = InternalTrace.GetLogger("TestBuilder");

        #region Instance Fields
        /// <summary>
        /// The loaded assembly
        /// </summary>
        Assembly assembly;

        /// <summary>
        /// Our LegacySuite builder, which is only used when a 
        /// fixture has been passed by name on the command line.
        /// </summary>
        ISuiteBuilder legacySuiteBuilder;

        #endregion

        #region Properties
        public Assembly Assembly
        {
            get { return assembly; }
        }

        #endregion

        #region Constructor

        public TestBuilder()
        {
            // TODO: Keeping this separate till we can make
            //it work in all situations.
            legacySuiteBuilder = new NUnit.Core.Builders.LegacySuiteBuilder();
        }

        #endregion

        #region Build Methods
        public Test Build(string assemblyName, string testName, bool autoSuites)
        {
            if (testName == null || testName == string.Empty)
                return Build(assemblyName, autoSuites);

            // Change currentDirectory in case assembly references unmanaged dlls
            // and so that any addins are able to access the directory easily.
            using (new DirectorySwapper(Path.GetDirectoryName(assemblyName)))
            {
                this.assembly = Load(assemblyName);
                if (assembly == null) return null;

                // If provided test name is actually the name of
                // a type, we handle it specially
                Type testType = assembly.GetType(testName);
                if (testType != null)
                    return Build(assemblyName, testType, autoSuites);

                // Assume that testName is a namespace and get all fixtures in it
                IList fixtures = GetFixtures(assembly, testName);
                if (fixtures.Count > 0)
                    return BuildTestAssembly(assemblyName, fixtures, autoSuites);

                return null;
            }
        }

        public TestSuite Build(string assemblyName, bool autoSuites)
        {
            // Change currentDirectory in case assembly references unmanaged dlls
            // and so that any addins are able to access the directory easily.
            using (new DirectorySwapper(Path.GetDirectoryName(assemblyName)))
            {
                this.assembly = Load(assemblyName);
                if (this.assembly == null) return null;

                IList fixtures = GetFixtures(assembly, null);
                return BuildTestAssembly(assemblyName, fixtures, autoSuites);
            }
        }

        private Test Build(string assemblyName, Type testType, bool autoSuites)
        {
            // TODO: This is the only situation in which we currently
            // recognize and load legacy suites. We need to determine 
            // whether to allow them in more places.
            if (legacySuiteBuilder.CanBuildFrom(testType))
                return legacySuiteBuilder.BuildFrom(testType);
            else if (TestFixtureBuilder.CanBuildFrom(testType))
                return BuildTestAssembly(assemblyName,
                    new Test[] { TestFixtureBuilder.BuildFrom(testType) }, autoSuites);
            return null;
        }

        private TestSuite BuildTestAssembly(string assemblyName, IList fixtures, bool autoSuites)
        {
            TestSuite testAssembly = new TestSuite(assemblyName);

            if (autoSuites)
            {
                NamespaceTreeBuilder treeBuilder =
                    new NamespaceTreeBuilder(testAssembly);
                treeBuilder.Add(fixtures);
                testAssembly = treeBuilder.RootSuite;
            }
            else
                foreach (TestSuite fixture in fixtures)
                {
                    if (fixture is SetUpFixture)
                    {
                        fixture.RunState = RunState.NotRunnable;
                        fixture.IgnoreReason = "SetUpFixture cannot be used when loading tests as a flat list of fixtures";
                    }

                    testAssembly.Add(fixture);
                }

            if (fixtures.Count == 0)
            {
                testAssembly.RunState = RunState.NotRunnable;
                testAssembly.IgnoreReason = "Has no TestFixtures";
            }

            NUnitFramework.ApplyCommonAttributes(assembly, testAssembly);

            testAssembly.Properties["_PID"] = System.Diagnostics.Process.GetCurrentProcess().Id;
            testAssembly.Properties["_APPDOMAIN"] = AppDomain.CurrentDomain.FriendlyName;


            // TODO: Make this an option? Add Option to sort assemblies as well?
            testAssembly.Sort();

            return testAssembly;
        }

        #endregion

        #region Helper Methods

        private Assembly Load(string path)
        {
            Assembly assembly = Assembly.LoadFrom(path);

            if (assembly != null)
                CoreExtensions.Host.InstallAdhocExtensions(assembly);

            log.Info("Loaded assembly " + assembly.FullName);

            return assembly;
        }

        private IList GetFixtures(Assembly assembly, string ns)
        {
            ArrayList fixtures = new ArrayList();
            log.Debug("Examining assembly for test fixtures");

            IList testTypes = GetCandidateFixtureTypes(assembly, ns);

            log.Debug("Found {0} classes to examine", testTypes.Count);
#if NET_2_0
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
#endif

            foreach (Type testType in testTypes)
            {
                if (TestFixtureBuilder.CanBuildFrom(testType))
                    fixtures.Add(TestFixtureBuilder.BuildFrom(testType));
            }

#if NET_2_0
            log.Debug("Found {0} fixtures in {1} seconds", fixtures.Count, timer.Elapsed);
#else
            log.Debug("Found {0} fixtures", fixtures.Count);
#endif

            return fixtures;
        }

        private IList GetCandidateFixtureTypes(Assembly assembly, string ns)
        {
            IList types = assembly.GetTypes();

            if (ns == null || ns == string.Empty || types.Count == 0)
                return types;

            string prefix = ns + ".";

            ArrayList result = new ArrayList();
            foreach (Type type in types)
                if (type.FullName.StartsWith(prefix))
                    result.Add(type);

            return result;
        }
        #endregion
    }
}


