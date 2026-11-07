# Head and Foot Development Conventions

## Purpose and scope

Use these conventions to make code clear, consistent, maintainable, and easy to
change. Reconsider a convention when a concrete problem warrants a different
approach; explain the tradeoff in the repository.

## Platform and language

- For a new solution, prefer the latest supported .NET Long Term Support (LTS)
  release and a modern C# version supported by its toolchain. Pin the chosen
  SDK and target framework in the repository where reproducibility matters.
- Honor existing runtime, platform, dependency, and consumer compatibility
  requirements. The C# language version and target framework are separate
  decisions; modern syntax does not imply that newer runtime APIs are
  available.
- Choose platform targets and application frameworks for the product. This
  document does not prescribe a UI technology, host type, operating system,
  or project layout.

## Design and boundaries

### Keep designs simple

Prefer the simplest design that meets current requirements. Introduce a layer,
interface, mapping, or pattern when it solves a concrete problem or protects
an intentional boundary. Avoid speculative architecture and classes created
only for symmetry.

Keep related behavior easy to find together. Organize by cohesive capability
when that improves [locality](https://htmx.org/essays/locality-of-behaviour/); use technical folders where they are clearer.
Expose deliberate contracts rather than implementation details.

### Separate responsibilities

Keep application rules and validation in the application or domain boundary.
Keep persistence mechanics, external protocols, and presentation concerns in
their respective implementations when those concerns exist. UI validation and
database constraints can supplement business validation but should not be its
sole enforcement.

Dependencies should point toward the contracts they use. Code that defines a
capability should not need to know the concrete database, provider, or UI
technology that implements it. An executable host or other composition root
may reference implementations to assemble the application. That technical
access does not grant permission to spread implementation behavior across
boundaries.

These are responsibility rules, not a required set of projects. A small
solution may keep boundaries within one project; split projects when
independent ownership, dependencies, reuse, or deployment justify it.

### Use only the representations needed

Distinguish a business concept from its stored representation and from a shape
made for an external or presentation contract. A database table does not
automatically define a business object, and a business concept need not have
one matching table, DTO, and UI model.

Use DTOs for intentional data transfer boundaries. Keep them simple and
serializable when serialization is required; records are often suitable.
Provider-specific and persistence-specific types should stay within their
respective boundaries unless deliberately exposed.

Prefer focused services or processes for nontrivial behavior when that makes
ownership clearer. Put behavior on an object when it meaningfully belongs
there. Do not require rich domain objects or service classes by default.

## Results, exceptions, and validation

Represent anticipated outcomes that callers must handle explicitly. A result
type, discriminated outcome, or `Try` pattern may fit operations with several
expected states. Use exceptions for unexpected failures and appropriate
programming or contract violations; do not use them as ordinary control flow.

Translate a known infrastructure or provider failure into an application
outcome at the boundary when that failure is expected by the contract. Preserve
diagnostic information for unexpected failures. Do not catch every exception
only to turn it into an undifferentiated failure result.

Validate input at the boundary that owns the rule. Keep validation close enough
to the operation that every caller receives the same enforcement.

## C# style

- Prefer file-scoped namespaces and place using directives outside the
  namespace. Use built-in aliases such as `int` and `string`.
- Prefer explicit local types with target-typed construction where clear:
  `Widget widget = new();`. Avoid the use of `var`.
- Use `PascalCase` for types and public or internal members, `camelCase` for
  parameters and locals, `_camelCase` for private instance fields, `s_camelCase`
  for private mutable static fields, and `UPPER_SNAKE_CASE` for constants and
  private static readonly fields.
- Prefer ordinary constructors for classes. Simple data records and DTOs may
  use primary constructors. Keep parameter lists on one line where readable.
- Always use braces for `if`, `else`, loops, `using` statements, and `lock`,
  including single-statement bodies. Put braces on separate lines (Allman
  style).
- Use block bodies for methods, constructors, operators, and local functions.
  Simple expression-bodied properties, indexers, and accessors are acceptable.
  Put each executable statement on its own line.
- Use four spaces, UTF-8, LF line endings, a final newline, and no trailing
  whitespace. Use `string.Empty` for an empty string.
- Otherwise follow the [Microsoft C# coding conventions](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions).
  Follow the [.NET library design guidelines](https://learn.microsoft.com/dotnet/standard/design-guidelines/)
  where applicable to public libraries.

Record enforceable formatting preferences in the repository's `.editorconfig`
where practical.

## HTTP Clients

Prefer `IHttpClientFactory` for code that
performs HTTP requests. Register HTTP client configuration and inject
`IHttpClientFactory` into long-lived consumers rather than registering
or capturing an unconfigured singleton `HttpClient`.

Create clients when an operation needs them:

``` csharp
HttpClient httpClient = _httpClientFactory.CreateClient();
```

Creating an `HttpClient` through the factory is inexpensive because the
factory manages and pools the underlying handlers. Named or typed
clients may be used when a concrete integration needs stable
provider-specific HTTP configuration.

A long-lived `HttpClient` is not inherently incorrect. It can be
appropriate when deliberately configured with connection-lifetime
management such as `SocketsHttpHandler.PooledConnectionLifetime`. Prefer
that approach only when its lifetime and handler policy are intentional
and provide a concrete advantage over the application's existing
`IHttpClientFactory` infrastructure.

Do not create a separate HTTP lifetime policy inside an integration
merely because it needs HTTP access. The composition root owns
application-level HTTP infrastructure; integration code owns
provider/protocol behavior.

## Object Vocabulary

### Persistent Entities / Data Entities

A **persistent entity** or **data entity** is a uniquely identifiable
persisted data representation. Use these qualified terms in
architectural discussion when ambiguity is possible.

Do not call every business object an entity merely because it has
identity.

Persistence structure does not define business structure. Multiple data
entities may support one business concept, and a data entity may exist
only for caching, synchronization, indexing, auditing, integration
state, or other infrastructure bookkeeping.

Persistent entities should generally remain simple data-oriented
objects.

### DTOs

DTOs are lightweight objects intended to transfer data across an
intentional boundary.

DTOs should:

-   Be serializable by design.
-   Be POCOs.
-   Generally be C# records.
-   Contain data rather than complex behavior.
-   Represent intended contract exposure rather than blindly mirror
persistence or provider representations.

External integrations may have provider-specific DTOs in addition to
app-facing DTOs. Do not expose an external provider DTO directly merely
to avoid writing a mapping.

### Domains / Business Concepts

A **domain** or **business concept** represents a real-world or
app-specific concept required by business logic. It is not defined by
whether or how it persists.

A business representation may have one persistent representation,
multiple persistent representations, or none.

Business objects should generally remain straightforward data-oriented
representations. Prefer focused business services/processes for
non-trivial behavior rather than automatically placing complex behavior
on the objects themselves.

Useful DDD ideas may be adopted when they solve a concrete problem, but
DDD terminology or rich domain objects are not required by default.

### Models

A **Model** is a representation intended for presentation consumption.
Models may represent a view of business concepts, DTOs, or composed
information from multiple sources.

In MVVM and MVC, Models are suitable for presentation consumption and
data binding. Presentation Models belong to Presentation when their
shape exists specifically for UI needs.

> Create a distinct representation only when a real boundary or transformation
> transformation requires it. KISS takes precedence over architectural ceremony.

## Working in a repository

- Read the repository's product and architecture documents before applying a
  general convention. Follow the chosen application structure and platform
  pattern.
- Add tests appropriate to the behavior and risk of a change. Verify the
  affected build and tests using the repository's documented workflow.
- When adapting substantial implementation or design from an identifiable
  source, give appropriate credit near the adopted work when practical.
  Ordinary techniques and general knowledge do not need mechanical citations.

> Architecture should make intended behavior easy to find, intended
> dependencies obvious, and unintended coupling difficult.
