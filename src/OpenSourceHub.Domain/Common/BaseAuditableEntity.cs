using System;

namespace OpenSourceHub.Domain.Common;

public class BaseAuditableEntity : BaseEntity
{
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
}
