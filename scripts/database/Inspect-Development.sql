-- Read-only inventory; run as administrative operator against postgres on port5433.
SELECT version(), current_database(), current_user, inet_server_port();
SELECT datname, pg_get_userbyid(datdba) AS owner, datallowconn FROM pg_database ORDER BY datname;
SELECT rolname, rolsuper, rolcreatedb, rolcreaterole, rolcanlogin, rolreplication, rolbypassrls
FROM pg_roles WHERE rolname = 'oneoffice_app';
SELECT roleid::regrole, member::regrole FROM pg_auth_members
WHERE member = (SELECT oid FROM pg_roles WHERE rolname = 'oneoffice_app');
