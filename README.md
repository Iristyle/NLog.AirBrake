# NLog.AirBrake #

An [NLog](http://nlog-project.org/) target that sends exception information to [AirBrake](https://airbrake.io/). We
are using the [SharpBrake](https://github.com/asbjornu/SharpBrake) library to communicate with AirBrake.

# Configuration #

You need to configure two things for this to work: NLog and SharpBrake

## NLog Configuration ##

Your NLog configuration should look something like this:

	<?xml version="1.0" encoding="utf-8" ?>
	<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
		  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
	  
	  <extensions>
		<add assembly="NLog.AirBrake"/> <!-- Add the assembly -->
	  </extensions>
	  <targets>
		<target xsi:type="AirBrake" name="airBrakeTarget" />  <!-- Set up the target -->
	  </targets>
	  <rules>
		<logger name="*" minlevel="Trace" writeTo="airBrakeTarget" />  <!-- Set up the logger. -->
	  </rules>
	</nlog>

## SharpBrake Configuration ##

Your app.config file should look something like this:

	<?xml version="1.0"?>
	<configuration>
		<appSettings>
			<add key="Airbrake.ApiKey" value="[Your API key here.]" />
			<add key="Airbrake.Environment" value="[Environment]" />
			<add key="Airbrake.ServerUri" value="[ServerApiUri]" />
		</appSettings>
	</configuration>

- [Environment] - the name of the server environment in which the error occurred, such as 'staging' or 'production'.
- [ServerApiUri] - the URI of the AirBrake API. For example: 
   - `http://api.airbrake.io/notifier_api/v2/notices`  (AirBrake)
   - `http://yourservername/notifier_api/v2/notices`  (Self-hosted ErrBit)
