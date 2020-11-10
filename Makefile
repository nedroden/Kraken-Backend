build:
	dotnet build

containers:
	docker-compose build --no-cache

dependencies:
	dotnet restore

run:
	docker-compose up -d

stop:
	docker-compose down
	
run_main:
	dotnet run --project=Main

run_sensor:
	dotnet run --project=Sensor

test:
	dotnet test

clean:
	rm -rf **/bin **/obj

config:
	cp Main/appsettings.json-example Main/appsettings.json
	cp Sensor/appsettings.json-example Sensor/appsettings.json

docs:
	doxygen Doxyfile