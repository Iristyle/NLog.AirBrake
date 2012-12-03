![Logo](https://github.com/EastPoint/NLog.AirBrake/raw/master/logo-128.png)

# NLog.AirBrake

An [NLog][NLog] target that sends exception information to [AirBrake][AirBrake]
API compatible listeners  This includes the hosted AirBrake service as well
as [Errbit][Errbit], the open-source Ruby alternative.

If you have the means to do so, please give AirBrake your money and use
their service!

We are using client code from [SharpBrake][SharpBrake] to handle communication
with AirBrake.

[NLog]: http://nlog-project.org/
[AirBrake]: https://airbrake.io/
[Errbit]: https://github.com/errbit/errbit
[SharpBrake]: https://github.com/asbjornu/SharpBrake

## Installation

Use the Nuget package manager to install [`NLog.AirBrake`][nlog.airbrake]

[nuget]: http://www.nuget.org
[nlog.airbrake]: http://nuget.org/packages/NLog.AirBrake

## Configuration

You need to configure two things for this to work: NLog and AirBrake

### NLog Configuration

Your `NLog.config` should look something like this:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

  <extensions>
    <!-- Add the assembly -->
    <add assembly="NLog.AirBrake" />
  </extensions>
  <targets>
    <!-- Set up the target -->
    <target xsi:type="AirBrake" name="airBrakeTarget" />
  </targets>
  <rules>
    <!-- Set up the logger. -->
    <logger name="*" minlevel="Trace" writeTo="airBrakeTarget" />
  </rules>
</nlog>
```

### AirBrake Configuration

Your `app.config` file should look something like this:

```xml
<?xml version="1.0"?>
<configuration>
	<appSettings>
		<add key="Airbrake.ApiKey" value="[API_Key]" />
		<add key="Airbrake.Environment" value="[Environment]" />
		<add key="Airbrake.ServerUri" value="[ServerApiUri]" />
	</appSettings>
</configuration>
```

The values in the above that should be replaced are

- `[API_Key]` - Self-explanatory
- `[Environment]` - The name of the server environment in which the error occurred, such as 'staging' or 'production'.
- `[ServerApiUri]` - The URI of the AirBrake API. For example:
    - `http://api.airbrake.io/notifier_api/v2/notices`  (AirBrake)
    - `http://yourservername/notifier_api/v2/notices`  (Self-hosted ErrBit)

### Securing configuration information

Its recommended that whenever credentials are stored in config, that
they be encrypted.  That is beyond the scope of this README, but some
information can be found [here][net-encrypt]

[net-encrypt]: http://msdn.microsoft.com/en-us/library/ff650304.aspx

## Release Notes

#### 0.5
* Added support for using target without exceptions


#### 0.2 - .NET 3.5

* Retargeted at .NET 3.5
* Added missing NLog and NLog.config packages to nuspec as dependencies

#### 0.1 - Initial release

* It works!

## Credits

* Thanks to [AirBrake][AirBrake] for standardizing a simple error notifications API, that others could leverage.
* Thanks to [SharpBrake][SharpBrake] for making their code open source
in a friendly MIT license that allows us to easily pull into our code.
You guys are good open source citizens!
* Thanks to [NLog][NLog] for the best open source logging framework for .NET
* Thanks to [Errbit][Errbit] for a great free piece of software to self-host
for cheapskates ;0
* The icon was derived from the published AirBrake and NLog logos -
hopefully it falls under derivative works licensing.

### Contributions

If you see something wrong, feel free to submit a pull request.
Coding guidelines are :

- Indent with 2 spaces
- 80 character lines
- Make sure the tests pass
