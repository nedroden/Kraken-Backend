CQL="CREATE KEYSPACE kraken WITH replication = {'class':'SimpleStrategy', 'replication_factor' : 3};
USE kraken;

CREATE TABLE house_measurements(
    id text primary key,
    house_id text,
    consumption float,
    created_at timestamp
);

CREATE TABLE pipe_measurements(
    id text primary key,
    pipe_id text,
    water_flow_volume float,
    water_quality float,
    created_at timestamp
);

CREATE TABLE source_measurements(
    id text primary key,
    source_id text,
    production float,
    water_quality float,
    created_at timestamp
);"

# A thousand thanks to: https://github.com/docker-library/cassandra/issues/104#issuecomment-413193337
until echo $CQL | cqlsh; do
    echo "cqlsh: Cassandra is unavailable to initialize - will retry later"
    sleep 2
done &

exec /docker-entrypoint.sh "$@"
