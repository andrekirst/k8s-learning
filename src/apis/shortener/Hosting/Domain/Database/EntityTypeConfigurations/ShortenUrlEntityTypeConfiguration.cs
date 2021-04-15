using System;
using Hosting.Domain.Database.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Hosting.Domain.Database.EntityTypeConfigurations
{
    public class ShortenUrlEntityTypeConfiguration : IEntityTypeConfiguration<ShortenUrl>
    {
        public void Configure(EntityTypeBuilder<ShortenUrl> builder)
        {
            builder
                .ToTable("ShortenUrl");

            builder
                .HasKey(s => s.Id);

            builder
                .Property(s => s.Id)
                .IsRequired()
                .HasValueGenerator<ValueGenerator<int>>();

            builder.Property(p => p.Code)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(10);

            builder
                .HasIndex(p => p.Code)
                .IsUnique();

            builder
                .Property(p => p.Url)
                .IsRequired()
                .HasMaxLength(2048);

            builder
                .Property(s => s.UrlHash)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(256);

            builder
                .HasIndex(p => p.UrlHash)
                .IsUnique();

            builder
                .Property(s => s.CreatedAt)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow)
                .HasAnnotation("Timezone", "UTC");
        }
    }
}