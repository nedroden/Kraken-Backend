# Kraken back-end

Backend code for the Kraken project, created for the Web and Cloud Computing course at the University of Groningen. **Note:** This repository only contains back-end code, since the front-end and simulator were created by other project members.

## Context

The backend consists of a sensor API and a main API. Both of these were built on top of .NET Core 3.1 and were written in the C# programming language. Due to an abundance of documentation and external plugins, support for all three major operating systems and the ability to use a staticly-typed programming language, the decision was reached to use .NET Core over alternatives such as NodeJS and Django/Flask. An additional factor that contributed to this decision was familiarity with .NET Core and the C# programming language.

The sensor API is responsible for reading sensor measurements from the message queue, saving these measurements to the database and then retransmitting the measurements to the frontend. Transmission to the frontend is accomplished through a websocket connection, which is initiated by the frontend. Sensor data is stored in a Cassandra database, which differs from the database system used by the main api since the sensor database needs to be capable of handling very large amounts of data. Possible alternatives would have included database systems such as Redis.

The main API is responsible for all other database interactions. It is connected to a MongoDB database and is able to defer certain requests (such as the latest measurements) to the sensor API. The main API also handles user login and registration. This is done using JWT tokens. To log in, users need to enter their credentials. If the credentials are correct, a new token is generated, taking into account the user's username, email address and id. In order to determine whether or not the user is logged in, the API automatically validates this token. While some endpoints, such as `GET /api/house` are public, endpoints such as `POST /api/house` or `PUT /api/house/abc-123-def-456` are not. Endpoints can easily be protected by applying the `[Authorize]` attribute onto the method corresponding to that endpoint. Originally (and ideally), endpoints would not just be protected through authentication, but also through authorization. This would have involved the creation of a permission system where each user would belong to one or more groups, which would in turn would have a set of permissions.


## Controllers, Models and Services

In the previous chapter, it was established that the main API and sensor API communicate, respectively, with a MongoDB database and a Cassandra database. Communication between controllers and the databases is done through services. On a high level, the way the two APIs approach this is quite similar. At the start of the program, services are instantiated. They are then injected (through built-in dependency injection) into the controller. When a user performs, say, a `GET`-request to `/api/area`, the corresponding controller method calls the corresponding service method.  The service then takes care of loading data from the database and returning the data as a model. In both APIs, database data structures are automatically mapped to API data structures (and vice versa). This means that, for example, column `user_id` in the database would be mapped to `UserId` in the model.

The sensor API uses a single service, called the `DataService`. This service contains a set of generic methods facilitating CRUD operations, meaning that house, pipe and source measurements can all be loaded using this service, assuming the correct model is provided (e.g. `_dataService.GetAll<House>()`).

In the main API, this is slightly different. Water infrastructure components such as pipes, sources and houses have different services. However, the implementation of CRUD operations for these services is done in a component super class.


## Endpoints

Since the sensor API would not be publicly accessible (it would be located inside a private network), all endpoints of the sensor API are public. These endpoints are:

**House measurements**
* GET /api/measurements/house
* GET /api/measurements/house/my-measurement-id
* GET /api/measurements/house/latest
* POST /api/measurements/house
* DELETE /api/measurements/house/my-measurement-id

**Pipe measurements**
* GET /api/measurements/pipe
* GET /api/measurements/pipe/my-measurement-id
* GET /api/measurements/pipe/latest
* POST /api/measurements/pipe
* DELETE /api/measurements/pipe/my-measurement-id

**Source measurements**
* GET /api/measurements/source
* GET /api/measurements/source/my-measurement-id
* GET /api/measurements/source/latest
* POST /api/measurements/source
* DELETE /api/measurements/source/my-measurement-id


The main API supports the following endpoints:

**Authentication**
* POST /api/login
* POST /api/register

**Measurements**
* GET /api/measurement/house
* GET /api/measurement/pipe
* GET /api/measurement/source

**Areas**
* GET /api/area
* GET /api/area/my-area-id
* POST /api/area (subject to authentication)
* PUT /api/area/my-area-id (subject to authentication)
* DELETE /api/area/my-area-id (subject to authentication)

**Houses**
* GET /api/house
* GET /api/house/my-house-id
* POST /api/house (subject to authentication)
* PUT /api/house/my-house-id (subject to authentication)
* DELETE /api/house/my-house-id (subject to authentication)

**Intersections**
* GET /api/intersection
* GET /api/intersection/my-intersection-id
* POST /api/intersection (subject to authentication)
* PUT /api/intersection/my-intersection-id (subject to authentication)
* DELETE /api/intersection/my-intersection-id (subject to authentication)

**Pipes**
* GET /api/pipe
* GET /api/pipe/my-pipe-id
* POST /api/pipe (subject to authentication)
* PUT /api/pipe/my-pipe-id (subject to authentication)
* DELETE /api/pipe/my-pipe-id (subject to authentication)

**Sources**
* GET /api/source
* GET /api/source/my-asourcerea-id
* POST /api/source (subject to authentication)
* PUT /api/source/my-source-id (subject to authentication)
* DELETE /api/source/my-source-id (subject to authentication)

**Streets**
* GET /api/street
* GET /api/street/my-street-id
* POST /api/street (subject to authentication)
* PUT /api/street/my-street-id (subject to authentication)
* DELETE /api/street/my-street-id (subject to authentication)


## Fault-Tolerance

Upon detection of database connection issues after a connection has previously been established, both APIs attempt to reconnect automatically. After connection with the database has been reestablished, a second attempt is made to complete the database transaction. This way, if a database container fails, the API would remain unaffected.

Unfortunately, it proved to difficult to accomplish this through attributes. The original idea was to add a `[ReconnectOnFailure]` attribute, this way methods would be wrapped inside try-catch blocks. This would negate the need for boilerplate code. However, since C# does not support this, it proved necessary to call the method manually. This means that instead of being able to do this `items.GetAll()`, it was necessary to write `ReconnectUponFailure<ItemType>(() => items.GetAll())`.


## Error Handling

The APIs are cable of performing some basic validation steps, however data validation by the APIs is severely limited. The reason behind this is that it would have taken too much time to implement this properly.

Each time an update is sent over the websocket connection, the sensor api ensures that the update is only sent to clients that are still connected. Clients that have disconnected, are removed from the list of clients. This ensures that disconnecting clients cannot cause the API to crash.


## Miscellaneous

While developing the APIs special attention was paid to the SOLID principles. The original idea was to write unit tests as well, however due to a lack of time this did not come to fruition.


## Documentation

Additional documentation can be generated by running `make docs`. This requires Doxygen to be installed and generates class documentation files inside the `api/Docs/html` directory.


## Run in Docker
The APIs were meant to run in Docker. Run `$ make containers` to build the containers and run `$ make run` to run them. **Note:** You do need to follow the installation steps described below, before the containers can be built. Specifically, the part about configuration. You also need to have Docker and Docker-Compose installed.

## Installation

In order to run a local copy of the APIs, the following tools/programs should be installed:

* Make
* .NET Core 3.1 or newer (tested on 3.1.108)
* Doxygen 1.8 or newer (dev only)

Follow these steps to run the APIs manually (i.e. outside of a Docker environment):

* Clone this repository
* Run `$ cd api`
* Run `$ make config`
* Enter the `Main` and `Sensor` directories and edit their respective `appsettings.json` file where needed
* Run `$ make dependencies`

Then, to run an API, run either `$ make run_main` or `$ make run_sensors`, depending on what API you want to run.

## Running unit tests

To run unit tests, run the following command inside the `api` directory: `$ make test`