using LMS.Models.LMSModels;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace ControllerUnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var optionsBuilder = new DbContextOptionsBuilder<Team56LMSContext>();
            optionsBuilder.UseInMemoryDatabase()
        }
    }
}
