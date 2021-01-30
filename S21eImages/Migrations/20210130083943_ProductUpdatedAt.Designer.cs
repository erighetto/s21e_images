﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using S21eImages;

namespace S21eImages.Migrations
{
    [DbContext(typeof(SQLiteDBContext))]
    [Migration("20210130083943_ProductUpdatedAt")]
    partial class ProductUpdatedAt
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("S21eImages.Model.Products", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Attempts")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Sku")
                        .HasColumnType("TEXT");

                    b.Property<int>("UpdatedAt")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
