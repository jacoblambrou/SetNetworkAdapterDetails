using NetworkAdapter.Library;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Xunit;

namespace NetworkAdapter.Library.Tests
{
    public class NetworkAdapterTest
    {
        [Fact()]
        public void GetAdapterNameTest()
        {
            // Arrange
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            var adapter = new NetworkAdapter();
            var expected = "Ethernet";

            // Act
            var actual = adapter.GetAdapterName(nics[0]);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
