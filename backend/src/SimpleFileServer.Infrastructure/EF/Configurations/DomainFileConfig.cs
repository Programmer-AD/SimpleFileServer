using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleFileServer.Domain;
using SimpleFileServer.Domain.Entities;

namespace SimpleFileServer.Infrastructure.EF.Configurations;

internal class DomainFileConfig : IEntityTypeConfiguration<DomainFile>
{
    public void Configure(EntityTypeBuilder<DomainFile> builder)
    {
        builder.HasKey(x => x.Id);

        builder.ToTable("T_DomainFiles");

        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(DomainEntityConstants.DomainFile_Name_MaxLength);

        builder
            .Property(x => x.FileLocation)
            .HasColumnName("file_location")
            .IsRequired()
            .HasMaxLength(DomainEntityConstants.DomainFile_FileLocation_MaxLength);

        builder
            .Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder
            .Property(x => x.Size)
            .HasColumnName("size")
            .IsRequired();
    }
}
