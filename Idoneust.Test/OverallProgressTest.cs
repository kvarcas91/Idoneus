using FluentAssertions;
using Idoneust.Test.DummyData;
using System;
using Xunit;

namespace Idoneust.Test
{

    public class OverallProgressTest
    {

        [Theory]
        [InlineData(1, 1, 1, 1, 100D)]
        [InlineData(1, 0, 0, 0, 0D)]
        [InlineData(1, 0, 1, 0, 0D)]
        [InlineData(1, 0, 1, 1, 100D)]
        [InlineData(1, 1, 0, 0, 100D)]
        [InlineData(1, 1, 1, 0, 100D)]
        [InlineData(2, 1, 0, 0, 50D)]
        [InlineData(2, 1, 2, 1, 75D)]
        [InlineData(3, 1, 0, 0, 33D)]
        [InlineData(0, 0, 0, 0, 0D)]
        public void OverallProgress(int tasCount, int taskCompletedCount, int subtaskCount, int subtaskCompletedCount, double expectedProgress)
        {

            var project = GetData.GetProject(new ValueTuple<int, int>(tasCount, taskCompletedCount), new ValueTuple<int, int>(subtaskCount, subtaskCompletedCount)); // 0
           
            double actualProgress = project.GetProgress();
            actualProgress.Should().Be(expectedProgress);
        }
    }
}
