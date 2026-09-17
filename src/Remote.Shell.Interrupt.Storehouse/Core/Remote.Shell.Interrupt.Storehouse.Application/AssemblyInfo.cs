using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
// Mediator.SourceGenerator (referenced by the API project) writes generated dispatch code
// that names handler types directly for performance (monomorphized Send, no reflection) -
// unlike MediatR's purely DI-based resolution, that requires compile-time access to our
// internal handler classes from the API assembly.
[assembly: InternalsVisibleTo("Remote.Shell.Interrupt.Storehouse.API")]
