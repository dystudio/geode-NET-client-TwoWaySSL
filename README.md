# Client Installation

	set PATH=C:\devtools\SSL\OpenSSL;%PATH%
	set LD_LIBRARY_PATH=C:\devtools\repositories\IMDG\pivotal-gemfire-native\lib  

	set LOCATOR_HOST=ec2-34-232-109-123.compute-1.amazonaws.com
	set SSL-KEYSTORE-PASSWORD=<PASSWORD>

Example command

	TwoWaySSL.exe

# Server Installation

## Login

ssh -i /Projects/Pivotal/AWS/operation/security/humana/data2-humana.pem  ec2-user@ec2-34-232-109-123.compute-1.amazonaws.com


## Set Environment Variables



export JAVA_ARGS="-Djavax.net.ssl.keyStoreType=pkcs12 -Djavax.net.ssl.trustStoreType=pkcs12 $JAVA_ARGS"

	export PATH=/runtime/java/bin:$PATH
	export JAVA_HOME=/runtime/java
	export JAVA_ARGS="-Djavax.net.ssl.keyStoreType=pkcs12 -Djavax.net.ssl.trustStoreType=pkcs12 $JAVA_ARGS"
	export GEMFIRE=/runtime/gemfire

———————————————————
## root pair

	 mkdir /root/ca

	  cd /root/ca
	 mkdir certs crl newcerts private
	 mkdir csr

	 chmod 700 private
	 touch index.txt
	 echo 1000 > serial


	 vi /root/ca/openssl.cnf


	 [ ca ]
	 # `man ca`
	 default_ca = CA_default



	 [ CA_default ]
	 # Directory and file locations.
	 dir               = /root/ca
	 certs             = $dir/certs
	 crl_dir           = $dir/crl
	 new_certs_dir     = $dir/newcerts
	 database          = $dir/index.txt
	 serial            = $dir/serial
	 RANDFILE          = $dir/private/.rand

	 # The root key and root certificate.
	 private_key       = $dir/private/ca.key.pem
	 certificate       = $dir/certs/ca.cert.pem

	 # For certificate revocation lists.
	 crlnumber         = $dir/crlnumber
	 crl               = $dir/crl/ca.crl.pem
	 crl_extensions    = crl_ext
	 default_crl_days  = 30

	 # SHA-1 is deprecated, so use SHA-2 instead.
	 default_md        = sha256

	 name_opt          = ca_default
	 cert_opt          = ca_default
	 default_days      = 375
	 preserve          = no
	 policy            = policy_strict


	 [ policy_strict ]
	 # The root CA should only sign intermediate certificates that match.
	 # See the POLICY FORMAT section of `man ca`.
	 countryName             = optional
	 stateOrProvinceName     = optional
	 organizationName        = optional
	 organizationalUnitName  = optional
	 commonName              = supplied
	 emailAddress            = optional


	 [ policy_loose ]
	 # Allow the intermediate CA to sign a more diverse range of certificates.
	 # See the POLICY FORMAT section of the `ca` man page.
	 countryName             = optional
	 stateOrProvinceName     = optional
	 localityName            = optional
	 organizationName        = optional
	 organizationalUnitName  = optional
	 commonName              = supplied
	 emailAddress            = optional


	 [ req ]
	 # Options for the `req` tool (`man req`).
	 default_bits        = 2048
	 distinguished_name  = req_distinguished_name
	 string_mask         = utf8only

	 # SHA-1 is deprecated, so use SHA-2 instead.
	 default_md          = sha256

	 # Extension to add when the -x509 option is used.
	 x509_extensions     = v3_ca


	 [ req_distinguished_name ]
	 # See <https://en.wikipedia.org/wiki/Certificate_signing_request>.
	 countryName                     = Country Name (2 letter code)
	 stateOrProvinceName             = State or Province Name
	 localityName                    = Locality Name
	 0.organizationName              = Organization Name
	 organizationalUnitName          = Organizational Unit Name
	 commonName                      = Common Name
	 emailAddress                    = Email Address

	 # Optionally, specify some defaults.
	 countryName_default             = GB
	 stateOrProvinceName_default     = England
	 localityName_default            =
	 0.organizationName_default      = Alice Ltd
	 #organizationalUnitName_default =
	 #emailAddress_default           =


	 [ v3_ca ]
	 # Extensions for a typical CA (`man x509v3_config`).
	 subjectKeyIdentifier = hash
	 authorityKeyIdentifier = keyid:always,issuer
	 basicConstraints = critical, CA:true
	 keyUsage = critical, digitalSignature, cRLSign, keyCertSign

	 [ v3_intermediate_ca ]
	 # Extensions for a typical intermediate CA (`man x509v3_config`).
	 subjectKeyIdentifier = hash
	 authorityKeyIdentifier = keyid:always,issuer
	 basicConstraints = critical, CA:true, pathlen:0
	 keyUsage = critical, digitalSignature, cRLSign, keyCertSign

	 [ usr_cert ]
	 # Extensions for client certificates (`man x509v3_config`).
	 basicConstraints = CA:FALSE
	 nsCertType = client, email
	 nsComment = "OpenSSL Generated Client Certificate"
	 subjectKeyIdentifier = hash
	 authorityKeyIdentifier = keyid,issuer
	 keyUsage = critical, nonRepudiation, digitalSignature, keyEncipherment
	 extendedKeyUsage = clientAuth, emailProtection


	 [ server_cert ]
	 # Extensions for server certificates (`man x509v3_config`).
	 basicConstraints = CA:FALSE
	 nsCertType = server
	 nsComment = "OpenSSL Generated Server Certificate"
	 subjectKeyIdentifier = hash
	 authorityKeyIdentifier = keyid,issuer:always
	 keyUsage = critical, digitalSignature, keyEncipherment
	 extendedKeyUsage = serverAuth


	 [ crl_ext ]
	 # Extension for CRLs (`man x509v3_config`).
	 authorityKeyIdentifier=keyid:always

	 [ ocsp ]
	 # Extension for OCSP signing certificates (`man ocsp`).
	 basicConstraints = CA:FALSE
	 subjectKeyIdentifier = hash
	 authorityKeyIdentifier = keyid,issuer
	 keyUsage = critical, digitalSignature
	 extendedKeyUsage = critical, OCSPSigning

## Make Authorative certificate

	 mkdir certs crl newcerts private

	 chmod 700 private

	 touch index.txt

	 echo 1000 > serial

	 openssl genrsa -aes256 -out private/ca.key.pem 4096

	 chmod 400 private/ca.key.pem

	 openssl req -config openssl.cnf   -key private/ca.key.pem   -new -x509 -days 7300 -sha256 -extensions v3_ca   -out certs/ca.cert.pem

	 Country Name (2 letter code) [GB]:US
	 State or Province Name [England]:Kentucky
	 Locality Name []:Louisville
	 Organization Name [Alice Ltd]:Pivotal
	 Organizational Unit Name []:Demo            
	 Common Name []:Pivotal GemFire demo
	 Email Address []:

	 chmod 444 certs/ca.cert.pem

	 openssl x509 -noout -text -in certs/ca.cert.pem


## Generate server cert

	ssh -i /Projects/Pivotal/AWS/operation/security/humana/data2-humana.pem ec2-user@34.232.109.123


	openssl genrsa -aes256       -out private/server-key.pem 2048

	chmod 400 private/server-key.pem

	openssl req -config openssl.cnf       -key private/server-key.pem       -new -sha256 -out csr/server-csr.pem

	openssl ca -config openssl.cnf -extensions server_cert -days 375 -notext -md sha256 -in csr/server-csr.pem -out certs/server-cert.pem

	chmod 444 certs/server-cert.pem

	openssl x509 -noout -text -in certs/server-cert.pem



## Generate client cert

	openssl genrsa -aes256       -out private/client-key.pem 2048

	chmod 400 private/client-key.pem

	openssl req -config openssl.cnf       -key private/client-key.pem       -new -sha256 -out csr/client-csr.pem

	openssl ca -config openssl.cnf  -extensions server_cert -days 375 -notext -md sha256 -in csr/client-csr.pem -out certs/client-cert.pem

	chmod 444 certs/client-cert.pem

	openssl x509 -noout -text   -in certs/client-cert.pem

———————————————————
## Generate pkcs12 file for server

	openssl pkcs12 -export -in certs/server-cert.pem -inkey private/server-key.pem -out server.p12 -name server  -CAfile ca.cert.pem -caname root

	Validate key store
	keytool -list -v -keystore server.p12

	———————————————————
	Generate pkcs12 file for client
	openssl pkcs12 -export -in certs/client-cert.pem -inkey private/client-key.pem -out client.p12 -name client  -CAfile ca.cert.pem -caname root

	———————————————————
	Generate certificate trust store
	keytool -import -alias root -keystore certificatetruststore -file certs/ca.cert.pem



## Copy Certificates

	 cp /root/ca/server.p12 /runtime/gem_cluster_1/
	 cp /root/ca/certificatetruststore /runtime/gem_cluster_1/
	 cp /root/ca/certs/ca.cert.pem .
	 cp /root/ca/certs/client-cert.pem .


	 cd /runtime/gem_cluster_1/
	 chown ec2-user:ec2-user gfsecurity.properties.backup2
	 chown ec2-user:ec2-user server.p12
	 chown ec2-user:ec2-user client-cert.pem
	 chown ec2-user:ec2-user ca.cert.pem


## vi gfsecurity.properties


	 ssl-enabled-components=all
	 ssl-require-authentication=true
	 ssl-keystore-type=pkcs12
	 ssl-keystore=/runtime/gem_cluster_1/server.p12
	 ssl-keystore-password=<password>
	 ssl-truststore=/runtime/gem_cluster_1/certificatetruststore
	 ssl-truststore-password=<password>


## Start Cluster


	start locator --dir=/runtime/gem_cluster_1/gem1101-locator --port=10000 --name=gem1101-locator --J=-Djava.rmi.server.hostname=ec2-34-232-109-123.compute-1.amazonaws.com --J="-Dgemfire.log-file-size-limit=10" --J="-Dgemfire.archive-disk-space-limit=100" --J="-Dgemfire.jmx-manager-start=true" --J="-Dgemfire.locators=ec2-34-232-109-123.compute-1.amazonaws.com[10000]" --J="-Dgemfire.jmx-manager-hostname-for-clients=ec2-34-232-109-123.compute-1.amazonaws.com" --J="-Dgemfire.distributed-system-id=1" --J="-Dgemfire.jmx-manager=true" --J="-Dgemfire.enable-network-partition-detection=true" --J="-Dgemfire.archive-file-size-limit=10" --J="-Dgemfire.jmx-manager-port=11099" --J="-Dgemfire.log-disk-space-limit=100" --J="-Dgemfire.statistic-sampling-enabled=true" --J="-Dgemfire.statistic-archive-file=locator.gfs" --J="-Dtype=locator" --J="-Dgemfire.http-service-port=17070" --J="-Dgemfire.log-level=config" --J=-Xmx2g --J=-Xms2g --J=-XX:+UseConcMarkSweepGC --J=-XX:+UseParNewGC --security-properties-file=/runtime/gem_cluster_1/gfsecurity.properties --J="-Djavax.net.ssl.trustStoreType=pkcs12" --J="-Djavax.net.ssl.keyStoreType=pkcs12"


	start server --dir=/runtime/gem_cluster_1/gem1101-server --name=gem1101-server --server-port=10100 --J="-Dgemfire.log-disk-space-limit=100" --J="-Dtype=datanode" --J="-Dgemfire.statistic-sampling-enabled=true" --J="-Dgemfire.conserve-sockets=False" --J="-Dgemfire.ALLOW_PERSISTENT_TRANSACTIONS=true" --J="-Dgemfire.locators=ec2-34-232-109-123.compute-1.amazonaws.com[10000]" --J="-Dgemfire.membership-port-range=10901-10999" --J="-Dgemfire.log-file-size-limit=10" --J="-Dgemfire.tcp-port=10001" --J="-Dgemfire.archive-disk-space-limit=100" --J="-Dgemfire.archive-file-size-limit=10" --J="-Dgemfire.enable-network-partition-detection=true" --J="-Dgemfire.log-level=config" --J="-Dgemfire.distributed-system-id=1" --J="-Dgemfire.statistic-archive-file=datanode.gfs" --J=-Xmx12500m --J=-Xms12500m --J=-Xmn1588m --J=-XX:+UseConcMarkSweepGC --J=-XX:+UseParNewGC --J=-XX:CMSInitiatingOccupancyFraction=85 --security-properties-file=/runtime/gem_cluster_1/gfsecurity.properties --J="-Djavax.net.ssl.trustStoreType=pkcs12" --J="-Djavax.net.ssl.keyStoreType=pkcs12"


## ------------------------------------------------
# Legacy Instructions

———————————————————

Generate client cert

openssl genrsa -aes256       -out private/client-key.pem 2048

chmod 400 private/client-key.pem

openssl req -config openssl.cnf       -key private/client-key.pem       -new -sha256 -out csr/client-csr.pem

openssl ca -config openssl.cnf \
      -extensions server_cert -days 375 -notext -md sha256 \
      -in csr/client-csr.pem \
      -out certs/client-cert.pem

chmod 444 certs/client-cert.pem

openssl x509 -noout -text \
      -in certs/client-cert.pem

———————————————————
Generate pkcs12 file for server
openssl pkcs12 -export -in server-cert.pem -inkey server-key.pem -out server.p12 -name server  -CAfile ca.cert.pem -caname root

Validate key store
keytool -list -v -keystore server.p12

———————————————————
Generate pkcs12 file for client
openssl pkcs12 -export -in client-cert.pem -inkey client-key.pem -out client.p12 -name client  -CAfile ca.cert.pem -caname root

———————————————————
Generate certificate trust store
keytool -import -alias root -keystore certificatetruststore -file ca.cert.pem




------------------

	connect --locators=ec2-34-232-109-123.compute-1.amazonaws.com[10000]



## Generate Certificates

	keytool -genkeypair -dname "cn=ec2-34-232-109-123.compute-1.amazonaws.com, ou=GemFire, o=humana, c=US" -storetype PKCS12 -keyalg RSA -keysize 2048 -alias gemfire1 -keystore gemfire1.keystore -storepass <password> -validity 180

	keytool -exportcert -storetype PKCS12 -keyalg RSA -keysize 2048 -alias gemfire1 -keystore gemfire1.keystore -storepass  -rfc -file gemfire1.cer

	keytool -importcert -alias gemfire1 -file gemfire1.cer -keystore certificatetruststore

	keytool -exportcert -alias gemfire1 -keystore gemfire1.keystore -rfc  -storetype pkcs12 -file gemfire1.pem -storepass <password>


------------

## Copy Certificates/Keystores

	scp -i /Projects/Pivotal/AWS/operation/security/humana/data2-humana.pem ec2-user@ec2-34-232-109-123.compute-1.amazonaws.com:/runtime/gem_cluster_1/gemfire1.cer /Projects/LifeSciences/Humana/dev/DigitalIT/NET/TwoWaySSL/cert/gemfire1.cer

	scp -i /Projects/Pivotal/AWS/operation/security/humana/data2-humana.pem  ec2-user@ec2-34-232-109-123.compute-1.amazonaws.com:/runtime/gem_cluster_1/gemfire1.pem /Projects/LifeSciences/Humana/dev/DigitalIT/NET/TwoWaySSL/cert/gemfire1.pem

	scp -i /Projects/Pivotal/AWS/operation/security/humana/data2-humana.pem  ec2-user@ec2-34-232-109-123.compute-1.amazonaws.com:/runtime/gem_cluster_1/gemfire1.keystore /Projects/LifeSciences/Humana/dev/DigitalIT/NET/TwoWaySSL/cert/gemfire1.keystore
	scp -i /Projects/Pivotal/AWS/operation/security/humana/data2-humana.pem  ec2-user@ec2-34-232-109-123.compute-1.amazonaws.com:/runtime/gem_cluster_1/certificatetruststore /Projects/LifeSciences/Humana/dev/DigitalIT/NET/TwoWaySSL/cert/certificatetruststore


 scp -i /Projects/Pivotal/AWS/operation/security/humana/data2-humana.pem /Projects/LifeSciences/Humana/dev/DigitalIT/NET/TwoWaySSL/cert/certificatetruststore ec2-user@ec2-34-232-109-123.compute-1.amazonaws.com:/runtime/gem_cluster_1



## vi gfsecurity.properties

 	ssl-enabled-components=all
	ssl-require-authentication=true
	ssl-keystore-type=pkcs12
	ssl-keystore=/runtime/gem_cluster_1/gemfire1.keystore
	ssl-keystore-password=<password>
	ssl-truststore=/runtime/gem_cluster_1/certificatetruststore
	ssl-truststore-password=<password>


	start locator --dir=/runtime/gem_cluster_1/gem1101-locator --port=10000 --name=gem1101-locator --J=-Djava.rmi.server.hostname=ec2-34-232-109-123.compute-1.amazonaws.com --J="-Dgemfire.log-file-size-limit=10" --J="-Dgemfire.archive-disk-space-limit=100" --J="-Dgemfire.jmx-manager-start=true" --J="-Dgemfire.locators=ec2-34-232-109-123.compute-1.amazonaws.com[10000]" --J="-Dgemfire.jmx-manager-hostname-for-clients=ec2-34-232-109-123.compute-1.amazonaws.com" --J="-Dgemfire.distributed-system-id=1" --J="-Dgemfire.jmx-manager=true" --J="-Dgemfire.enable-network-partition-detection=true" --J="-Dgemfire.archive-file-size-limit=10" --J="-Dgemfire.jmx-manager-port=11099" --J="-Dgemfire.log-disk-space-limit=100" --J="-Dgemfire.statistic-sampling-enabled=true" --J="-Dgemfire.statistic-archive-file=locator.gfs" --J="-Dtype=locator" --J="-Dgemfire.http-service-port=17070" --J="-Dgemfire.log-level=config" --J=-Xmx2g --J=-Xms2g --J=-XX:+UseConcMarkSweepGC --J=-XX:+UseParNewGC --security-properties-file=/runtime/gem_cluster_1/gfsecurity.properties --J="-Djavax.net.ssl.trustStoreType=pkcs12" --J="-Djavax.net.ssl.keyStoreType=pkcs12"


	start server --dir=/runtime/gem_cluster_1/gem1101-server --name=gem1101-server --server-port=10100 --J="-Dgemfire.log-disk-space-limit=100" --J="-Dtype=datanode" --J="-Dgemfire.statistic-sampling-enabled=true" --J="-Dgemfire.conserve-sockets=False" --J="-Dgemfire.ALLOW_PERSISTENT_TRANSACTIONS=true" --J="-Dgemfire.locators=ec2-34-232-109-123.compute-1.amazonaws.com[10000]" --J="-Dgemfire.membership-port-range=10901-10999" --J="-Dgemfire.log-file-size-limit=10" --J="-Dgemfire.tcp-port=10001" --J="-Dgemfire.archive-disk-space-limit=100" --J="-Dgemfire.archive-file-size-limit=10" --J="-Dgemfire.enable-network-partition-detection=true" --J="-Dgemfire.log-level=config" --J="-Dgemfire.distributed-system-id=1" --J="-Dgemfire.statistic-archive-file=datanode.gfs" --J=-Xmx12500m --J=-Xms12500m --J=-Xmn1588m --J=-XX:+UseConcMarkSweepGC --J=-XX:+UseParNewGC --J=-XX:CMSInitiatingOccupancyFraction=85 --security-properties-file=/runtime/gem_cluster_1/gfsecurity.properties --J="-Djavax.net.ssl.trustStoreType=pkcs12" --J="-Djavax.net.ssl.keyStoreType=pkcs12"
