// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using Xunit;
using RabbitMQ.Client;
using NSubstitute;
using System.Linq;

namespace EasyNetQ.Tests.ConsumeTests
{
    public class When_consume_is_called : ConsumerTestBase
    {
        protected override void AdditionalSetUp()
        {
            StartConsumer((body, properties, info) => { });
        }

        [Fact]
        public void Should_create_a_consumer()
        {
            MockBuilder.Consumers.Count.ShouldEqual(1);
        }

        [Fact]
        public void Should_create_a_channel_to_consume_on()
        {
            MockBuilder.Channels.Count.ShouldEqual(1);
        }

        [Fact]
        public void Should_invoke_basic_consume_on_channel()
        {
            MockBuilder.Channels[0].Received().BasicConsume(
               Arg.Is("my_queue"),
               Arg.Is(false), // NoAck
               Arg.Is(ConsumerTag),
               Arg.Is(true),
               Arg.Is(false),
               Arg.Is<IDictionary<string, object>>(x => x.SequenceEqual(new Dictionary <string, object>
                   {
                        {"x-priority", 0},
                        {"x-cancel-on-ha-failover", false}
                   })),
               Arg.Is(MockBuilder.Consumers[0]));
        }

        [Fact]
        public void Should_write_debug_message()
        {
            MockBuilder.Logger.Received().InfoWrite(
                                                   "Declared Consumer. queue='{0}', consumer tag='{1}' prefetchcount={2} priority={3} x-cancel-on-ha-failover={4}",
                                                   "my_queue",
                                                   ConsumerTag,
                                                   (ushort)50,
                                                   0,
                                                   false);
        }
    }
}