using Amqp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using Kraken.Api.Sensor.Controllers;
using Kraken.Api.Sensor.Models;
using Kraken.Api.Sensor.Services;

namespace Kraken.Api.Sensor
{
    /// <summary>
    /// Represents a set of measurements.
    /// </summary>
    public class MeasurementSet
    {
        /// <value>The time at which the measurements were taken.</value>
        public long Timestamp { get; set; }

        /// <value>House measurements.</value>
        public List<House> Houses { get; set; }

        /// <value>Pipe measurements.</value>
        public List<Pipe> Pipes { get; set; }

        /// <value>Source measurements.</value>
        public List<Source> Sources { get; set; }

        /// <summary>
        /// Injects the time into each measurement.
        /// </summary>
        /// <param name="date">The time at which the measurement was taken.</param>
        public void AddDates(DateTime date)
        {
            Houses.ForEach((House house) => house.CreatedAt = date);
            Pipes.ForEach((Pipe pipe) => pipe.CreatedAt = date);
            Sources.ForEach((Source source) => source.CreatedAt = date);
        }
    }

    /// <summary>
    /// ACtiveMQ queue consumer.
    /// </summary>
    public class Consumer
    {
        private ReceiverLink _receiver;
        private Session _session;
        private Connection _connection;

        private DataService _dataService;
        private WebSocketService _webSocketService;

        /// <summary>
        /// Creates a new consumer.
        /// </summary>
        /// <param name="dataService">The data service.</param>
        /// <param name="webSocketService">The websocket service.</param>
        public Consumer(DataService dataService, WebSocketService webSocketService)
        {
            _dataService = dataService;
            _webSocketService = webSocketService;
        }

        /// <value>Determines whether or not the consumer should continue listening.</value>
        public bool HasStopped { get; private set; } = true;

        /// <summary>
        /// Connects to the queue.
        /// </summary>
        /// <param name="connectionString">The queue connection string.</param>
        /// <param name="queue">The queue name.</param>
        public void Connect(string connectionString, string queue)
        {
            var address = new Address(connectionString);

            _connection = new Connection(address);
            _session = new Session(_connection);
            _receiver = new ReceiverLink(_session, "measurement-link", queue);
        }

        /// <summary>
        /// Starts listening to the queue and accepts messages as they enter the queue.
        /// </summary>
        public void Listen()
        {
            Message message;
            HasStopped = false;

            while (!HasStopped)
            {
                message = _receiver.Receive();

                if (message != null)
                {
                    _receiver.Accept(message);
                    HandleUpdate(message);
                }
            }
        }

        /// <summary>
        /// Handles the receival of a queue message.
        /// </summary>
        /// <param name="message">The message that was read.</param>
        private void HandleUpdate(Message message)
        {
            MeasurementSet measurements = JsonConvert.DeserializeObject<MeasurementSet>(message.Body.ToString());

            // https://stackoverflow.com/questions/249760/how-can-i-convert-a-unix-timestamp-to-datetime-and-vice-versa
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(measurements.Timestamp);
            measurements.AddDates(dateTime);

            Persist(measurements);
            SendUpdateToClients(measurements);
        }

        /// <summary>
        /// Stores a set of measurements in the database.
        /// </summary>
        /// <param name="measurements">The set of measurements.</param>
        private void Persist(MeasurementSet measurements)
        {
            var houseController = new HouseController(_dataService);
            var pipeController = new PipeController(_dataService);
            var sourceController = new SourceController(_dataService);

            measurements.Houses.ForEach((House house) => houseController.Create(house));
            measurements.Pipes.ForEach((Pipe pipe) => pipeController.Create(pipe));
            measurements.Sources.ForEach((Source source) => sourceController.Create(source));
        }

        /// <summary>
        /// Notifies clients that there was a new measurement received.
        /// </summary>
        /// <param name="measurementSet">The set of measurements.</param>
        private void SendUpdateToClients(MeasurementSet measurementSet)
        {
            _webSocketService.NotifyClients<MeasurementSet>(measurementSet);
        }

        /// <summary>
        /// Disconnects from the queue.
        /// </summary>
        public void Disconnect()
        {
            HasStopped = true;

            if (_connection != null)
            {
                _receiver.Close();
                _session.Close();
                _connection.Close();
            }
        }
    }
}