﻿using MultiWorldTesting;
using Newtonsoft.Json;
using System;

namespace ClientDecisionService
{
    /// <summary>
    /// Configuration object for the client decision service which contains settings for batching, retry storage, etc...
    /// </summary>
    public class DecisionServiceConfiguration<TContext>
    {
        // TODO: add xml comment once it's decided what to do with the token in offline mode.
        public DecisionServiceConfiguration(string authorizationToken, IExplorer<TContext> explorer)
        {
            if (authorizationToken == null)
            {
                throw new ArgumentNullException("connectionString", "Authorization token cannot be null");
            }

            if (explorer == null)
            {
                throw new ArgumentNullException("explorer", "Exploration algorithm cannot be null");
            }

            this.AuthorizationToken = authorizationToken;
            this.Explorer = explorer;
        }

        public DecisionServiceConfiguration(string connectionString, IExplorer<TContext> explorer)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString", "Connection string cannot be null");
            }

            if (explorer == null)
            {
                throw new ArgumentNullException("explorer", "Exploration algorithm cannot be null");
            }

            this.AzureStorageConnectionString = connectionString;
            this.Explorer = explorer;
        }

        /// <summary>
        /// The authorization token that is used for request authentication.
        /// </summary>
        public string AuthorizationToken { get; private set; }

        /// <summary>
        /// The connection string for Azure storage where settings and models are stored.
        /// </summary>
        public string AzureStorageConnectionString { get; private set; }

        /// <summary>
        /// The <see cref="IExplorer{TContext}"/> object representing an exploration algorithm.
        /// </summary>
        public IExplorer<TContext> Explorer { get; private set; }

        #region Optional Parameters

        /// <summary>
        /// Indicates whether to operate in offline mode where polling and join service logging are turned off.
        /// </summary>
        /// <remarks>
        /// In offline mode, a custom <see cref="IRecorder{TContext}"/> object must be defined.
        /// </remarks>
        public bool OfflineMode { get; set; }

        /// <summary>
        /// Specifies a custom <see cref="IRecorder{TContext}"/> object to be used for logging exploration data. 
        /// </summary>
        public IRecorder<TContext> Recorder 
        { 
            get { return recorder; } 
            set 
            { 
                if (value == null) throw new ArgumentNullException("Recorder cannot be null");
                recorder = value;
            } 
        }

        /// <summary>
        /// Specifies the output directory on disk for blob download (e.g. of settings or model files).
        /// </summary>
        public string BlobOutputDir 
        { 
            get { return blobOutputDir; } 
            set 
            { 
                if (value == null) throw new ArgumentNullException("Blob output directory cannot be null");
                blobOutputDir = value;
            } 
        }
        
        /// <summary>
        /// Specifies the batching configuration when uploading data to join service.
        /// </summary>
        /// <remarks>
        /// In offline mode, batching configuration will not be used since the join service recorder is turned off.
        /// </remarks>
        public BatchingConfiguration JoinServiceBatchConfiguration 
        {
            get { return batchConfig; }
            set 
            { 
                if (value == null) throw new ArgumentNullException("Batch configuration cannot be null");
                batchConfig = value;
            } 
        }
        
        // TODO: is this needed for v1?
        /// <summary>
        /// A custom serializer for the context.
        /// </summary>
        public Func<TContext, string> ContextJsonSerializer 
        {
            get { return contextJsonSerializer; }
            set 
            { 
                if (value == null) throw new ArgumentNullException("Custom JSON serializer cannot be null");
                contextJsonSerializer = value;
            } 
        }
        
        /// <summary>
        /// Specifies the address for a custom HTTP logging service.
        /// </summary>
        /// <remarks>
        /// When specified, this will override the default join service logging provided by the Multiworld Testing Service.
        /// </remarks>
        public string LoggingServiceAddress 
        {
            get { return loggingServiceAddress; }
            set 
            { 
                if (String.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("Logging service address cannot be empty");
                loggingServiceAddress = value;
            } 
        }
        
        /// <summary>
        /// Specifies the address for a custom HTTP command center.
        /// </summary>
        /// <remarks>
        /// Command center is responsible for storing & distributing centralized config settings across different clients.
        /// When specified, this will override the default command center provided by the Multiworld Testing Service.
        /// </remarks>
        public string CommandCenterAddress 
        {
            get { return commandCenterAddress; }
            set 
            { 
                if (String.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("Command center address cannot be empty");
                commandCenterAddress = value;
            } 
        }

        /// <summary>
        /// Specifies the polling period to check for updated application settings.
        /// </summary>
        /// <remarks>
        /// Polling is turned off if this value is set to <see cref="TimeSpan.MinValue"/>.
        /// </remarks>
        public TimeSpan PollingForSettingsPeriod
        {
            get { return pollingForSettingsPeriod; }
            set 
            {
                if (value <= TimeSpan.FromSeconds(0)) throw new ArgumentNullException("Invalid polling period value.");
                pollingForSettingsPeriod = value;
            }
        }

        /// <summary>
        /// Specifies the polling period to check for updated ML model.
        /// </summary>
        /// /// <remarks>
        /// Polling is turned off if this value is set to <see cref="TimeSpan.MinValue"/>.
        /// </remarks>
        public TimeSpan PollingForModelPeriod
        {
            get { return pollingForModelPeriod; }
            set
            {
                if (value <= TimeSpan.FromSeconds(0)) throw new ArgumentNullException("Invalid polling period value.");
                pollingForModelPeriod = value;
            }
        }

        public Action<Exception> ModelPollFailureCallback { get; set; }
        public Action<Exception> SettingsPollFailureCallback { get; set; }

        private IRecorder<TContext> recorder;
        private string blobOutputDir;
        private BatchingConfiguration batchConfig;
        private Func<TContext, string> contextJsonSerializer;
        private string loggingServiceAddress;
        private string commandCenterAddress;
        private TimeSpan pollingForSettingsPeriod;
        private TimeSpan pollingForModelPeriod;

        #endregion
    }
}
