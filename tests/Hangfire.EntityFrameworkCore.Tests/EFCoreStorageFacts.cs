﻿using System;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Hangfire.EntityFrameworkCore.Tests
{
    public class EFCoreStorageFacts : EFCoreStorageTest
    {
        [Fact]
        public void Ctor_Throws_WhenContextOptionsActionParameterIsNull()
        {
            Action<DbContextOptionsBuilder> optionsAction = null;
            var options = new EFCoreStorageOptions();

            Assert.Throws<ArgumentNullException>(nameof(optionsAction),
                () => new EFCoreStorage(optionsAction, options));
        }

        [Fact]
        public void Ctor_Throws_WhenOptionsParameterIsNull()
        {
            var contextOptions = OptionsActionStub;
            EFCoreStorageOptions options = null;

            Assert.Throws<ArgumentNullException>(nameof(options),
                () => new EFCoreStorage(OptionsAction, options));
        }

        [Fact]
        public void Ctor_CreatesInstance()
        {
            var options = new EFCoreStorageOptions
            {
                DistributedLockTimeout = new TimeSpan(1, 0, 0),
                QueuePollInterval = new TimeSpan(0, 1, 0)
            };

            var instance = new EFCoreStorage(OptionsActionStub, options);

            Assert.NotNull(Assert.IsType<DbContextOptions<HangfireContext>>(
                instance.GetFieldValue("_contextOptions")));
            Assert.Same(options, Assert.IsType<EFCoreStorageOptions>(
                instance.GetFieldValue("_options")));
            Assert.Equal(options.DistributedLockTimeout, instance.DistributedLockTimeout);
            Assert.Equal(options.QueuePollInterval, instance.QueuePollInterval);
        }

        [Fact]
        public void GetConnection_ReturnsCorrectResult()
        {
            var options = new EFCoreStorageOptions();
            var instance = new EFCoreStorage(OptionsAction, options);

            var result = instance.GetConnection();

            Assert.NotNull(result);
            var connection = Assert.IsType<EFCoreStorageConnection>(result);

            Assert.Same(instance,
                Assert.IsType<EFCoreStorage>(
                    connection.GetFieldValue("_storage")));
        }

        [Fact]
        public void GetMonitoringApi_ReturnsCorrectResult()
        {
            var options = new EFCoreStorageOptions();
            var instance = new EFCoreStorage(OptionsAction, options);

            var result = instance.GetMonitoringApi();

            Assert.NotNull(result);
            var api = Assert.IsType<EFCoreStorageMonitoringApi>(result);

            Assert.Same(instance,
                Assert.IsType<EFCoreStorage>(result.GetFieldValue("_storage")));
        }

        [Fact]
        public void CreateContext_CreatesInstance()
        {
            var instance = Storage.CreateContext();
            Assert.NotNull(instance);
            instance.Dispose();
        }

        [Fact]
        public void UseContext_Throws_WhenActionParameterIsNull()
        {
            Action<HangfireContext> action = null;

            Assert.Throws<ArgumentNullException>(nameof(action),
                () => Storage.UseContext(action));
        }

        [Fact]
        public void UseContext_InvokesAction()
        {
            bool exposed = false;
            Action<HangfireContext> action = context => exposed = true;

            Storage.UseContext(action);

            Assert.True(exposed);
        }

        [Fact]
        public void UseContextGeneric_Throws_WhenFuncParameterIsNull()
        {
            Func<HangfireContext, bool> func = null;

            Assert.Throws<ArgumentNullException>(nameof(func),
                () => Storage.UseContext(func));
        }

        [Fact]
        public void UseContextGeneric_InvokesFunc()
        {
            bool exposed = false;
            Func<HangfireContext, bool> func = context => exposed = true;

            var result = Storage.UseContext(func);

            Assert.True(exposed);
            Assert.True(result);
        }
    }
}
