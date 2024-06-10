namespace Remote.Shell.Interrupt.SNMPExecutor.Tests.Validators;

public class SNMPGetWalkCommandHandlerValidatorTests
{
  readonly SNMPGetWalkRequestValidator _validator;
  SNMPGetWalkRequest? _request;
  readonly string _host;
  readonly string _community;
  readonly string _oid;

  public SNMPGetWalkCommandHandlerValidatorTests()
    => (_validator, _host, _community, _oid)
    = (new SNMPGetWalkRequestValidator(),
       "192.168.1.1",
       "public",
       "1.3.6.1.2.1.1.1.0");

  [Test]
  public async Task Should_Not_Have_Error_When_Model_Is_Valid()
  {
    // Arrange
    _request = new SNMPGetWalkRequest(_host, _community, _oid);

    // Act
    var result = await _validator.TestValidateAsync(_request);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Host);
    result.ShouldNotHaveValidationErrorFor(x => x.Community);
    result.ShouldNotHaveValidationErrorFor(x => x.OID);
  }

  [Test]
  public async Task Should_Have_Error_When_Host_Is_Null()
  {
    // Arrange
    _request = new SNMPGetWalkRequest(null, _community, _oid);

    // Act
    var result = await _validator.TestValidateAsync(_request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Host);
  }

  [Test]
  public async Task Should_Have_Error_When_Host_Is_Empty()
  {
    // Arrange
    _request = new SNMPGetWalkRequest("", _community, _oid);

    // Act
    var result = await _validator.TestValidateAsync(_request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Host);
  }

  [Test]
  public async Task Should_Have_Error_When_Host_Is_Not_Valid()
  {
    // Arrange
    _request = new SNMPGetWalkRequest("999.999.999.999", _community, _oid);

    // Act
    var result = await _validator.TestValidateAsync(_request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Host);
  }

  [Test]
  public async Task Should_Have_Error_When_Community_Is_Null()
  {
    // Arrange
    _request = new SNMPGetWalkRequest(_host, null, _oid);

    // Act
    var result = await _validator.TestValidateAsync(_request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Community);
  }

  [Test]
  public async Task Should_Have_Error_When_Community_Is_Empty()
  {
    // Arrange
    _request = new SNMPGetWalkRequest(_host, "", _oid);

    // Act
    var result = await _validator.TestValidateAsync(_request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Community);
  }

  [Test]
  public async Task Should_Have_Error_When_OID_Is_Null()
  {
    // Arrange
    _request = new SNMPGetWalkRequest(_host, _community, null);

    // Act
    var result = await _validator.TestValidateAsync(_request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.OID);
  }

  [Test]
  public async Task Should_Have_Error_When_OID_Is_Empty()
  {
    // Arrange
    _request = new SNMPGetWalkRequest(_host, _community, "");

    // Act
    var result = await _validator.TestValidateAsync(_request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.OID);
  }

  [Test]
  public async Task Should_Have_Error_When_OID_Is_Not_Valid()
  {
    // Arrange
    _request = new SNMPGetWalkRequest(_host, _community, "a.b.1.3");

    // Act
    var result = await _validator.TestValidateAsync(_request);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.OID);
  }
}
