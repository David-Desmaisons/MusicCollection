using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NUnit;
using NUnit.Framework;
using FluentAssertions;

using FakeItEasy;
using NSubstitute;

using MusicCollection.Infra;
using MusicCollection.Fundation;
using MusicCollectionTest.TestObjects;
using System.ServiceModel;
using MusicCollection.Infra.Communication;

namespace MusicCollectionTest.Infra
{
    [ServiceContract]
    public interface ITheSame
    { 
        [OperationContract]
        int Get(int iEntrtry);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class MySame : ITheSame
    {
        public int Get(int iEntrtry)
        {
            return iEntrtry;
        }
    }

    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("Infra")]
    public class CommunicationTest
    {

        private IPCServer<ITheSame> _Server = null;

        [TearDown]
        public void CleanUp()
        {
            if (_Server != null)
            {
                _Server.Dispose();
                _Server = null;
            }
        }


        [Test]
        public void Test_OK_IsOK()
        {
            MySame ms = new MySame();
            _Server = new IPCServer<ITheSame>("Test",ms, "Test");

            IPCClient<ITheSame> target = new IPCClient<ITheSame>("Test", "Test");
            ITheSame res = target.GetService();

            res.Should().NotBeNull();
            res.Get(9).Should().Be(9);
            res.Get(10).Should().Be(10);
        }

        [Test]
        public void Test_NotFound_WrongAdress()
        {
            MySame ms = new MySame();
            _Server = new IPCServer<ITheSame>("Test", ms, "Test");

            IPCClient<ITheSame> target = new IPCClient<ITheSame>("Test2", "Test");
            ITheSame res = target.GetService();

            res.Should().NotBeNull();

            int resget = 0;
            Action ac = () => resget =  res.Get(0);
            ac.ShouldThrow<EndpointNotFoundException>();
           
        }

        [Test]
        public void Test_NotFound_WrongName()
        {
            MySame ms = new MySame();
            _Server = new IPCServer<ITheSame>("Test", ms, "Test");

            IPCClient<ITheSame> target = new IPCClient<ITheSame>("Test", "Test2");
            ITheSame res = target.GetService();

            int resget = 0;
            Action ac = () => resget = res.Get(0);
            ac.ShouldThrow<EndpointNotFoundException>();
        }

        [Test]
        public void Test_NotFound_NotRunned()
        {
            IPCClient<ITheSame> target = new IPCClient<ITheSame>("Test2", "Test");
            ITheSame res = target.GetService();

            int resget = 0;
            Action ac = () => resget = res.Get(0);
            ac.ShouldThrow<EndpointNotFoundException>();
        }

        [Test]
        public void Test_OK_IsOK_SimpleForm()
        {
            MySame ms = new MySame();
            _Server = new IPCServer<ITheSame>(ms,"Test");

            IPCClient<ITheSame> target = new IPCClient<ITheSame>("Test");
            ITheSame res = target.GetService();

            res.Should().NotBeNull();
            res.Get(9).Should().Be(9);
            res.Get(10).Should().Be(10);
        }
    }
}
