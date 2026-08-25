using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using dotNetAssignment.Data;
using dotNetAssignment.Models.Entities;
using dotNetAssignment.Repositories.Jwt;
using dotNetAssignment.Tests.Helpers;

namespace dotNetAssignment.Tests.Repositories.Jwt
{
    [TestFixture]
    public class JwtRepositoryTests
    {
        private Mock<RestaurantDbContext> _context;
        private Mock<DbSet<RefreshToken>> _refreshTokens;
        private JwtRepository _repository;

        [SetUp]
        public void Setup()
        {
            _context = new Mock<RestaurantDbContext>();
            _refreshTokens = new Mock<DbSet<RefreshToken>>();

            _context
                .Setup(x => x.RefreshTokens)
                .Returns(_refreshTokens.Object);

            _repository = new JwtRepository(_context.Object);
        }

        private void SetupAsyncDbSet(IQueryable<RefreshToken> data)
        {
            var mockSet = new Mock<DbSet<RefreshToken>>();

            mockSet.As<IDbAsyncEnumerable<RefreshToken>>()
                .Setup(x => x.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<RefreshToken>(
                    data.GetEnumerator()));

            mockSet.As<IQueryable<RefreshToken>>()
                .Setup(x => x.Provider)
                .Returns(new TestDbAsyncQueryProvider<RefreshToken>(
                    data.Provider));

            mockSet.As<IQueryable<RefreshToken>>()
                .Setup(x => x.Expression)
                .Returns(data.Expression);

            mockSet.As<IQueryable<RefreshToken>>()
                .Setup(x => x.ElementType)
                .Returns(data.ElementType);

            mockSet.As<IQueryable<RefreshToken>>()
                .Setup(x => x.GetEnumerator())
                .Returns(() => data.GetEnumerator());

            _context
                .Setup(x => x.RefreshTokens)
                .Returns(mockSet.Object);
        }

        [Test]
        public async Task JwtIdExistsAsync_WhenJwtIdExists_ReturnsTrue()
        {
            var jwtId = Guid.NewGuid();

            var data = new List<RefreshToken>
            {
                new RefreshToken
                {
                    JwtId = jwtId,
                    CreatedAt = DateTime.UtcNow
                }
            }.AsQueryable();

            SetupAsyncDbSet(data);

            var result = await _repository.JwtIdExistsAsync(jwtId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task JwtIdExistsAsync_WhenJwtIdDoesNotExist_ReturnsFalse()
        {
            var searchedJwtId = Guid.NewGuid();

            var data = new List<RefreshToken>
            {
                new RefreshToken
                {
                    JwtId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                }
            }.AsQueryable();

            SetupAsyncDbSet(data);

            var result = await _repository.JwtIdExistsAsync(searchedJwtId);

            Assert.That(result, Is.False);
        }

        [Test]
        public void AddJwtId_AddsRefreshToken()
        {
            var jwtId = Guid.NewGuid();

            _repository.AddJwtId(jwtId);

            _refreshTokens.Verify(
                x => x.Add(
                    It.Is<RefreshToken>(
                        token => token.JwtId == jwtId)),
                Times.Once);
        }

        [Test]
        public async Task RemoveJwtIdAsync_WhenTokenExists_RemovesToken()
        {
            var jwtId = Guid.NewGuid();

            var token = new RefreshToken
            {
                JwtId = jwtId
            };

            _refreshTokens
                .Setup(x => x.FindAsync(jwtId))
                .ReturnsAsync(token);

            await _repository.RemoveJwtIdAsync(jwtId);

            _refreshTokens.Verify(
                x => x.Remove(token),
                Times.Once);
        }

        [Test]
        public async Task RemoveJwtIdAsync_WhenTokenDoesNotExist_DoesNotRemoveToken()
        {
            var jwtId = Guid.NewGuid();

            _refreshTokens
                .Setup(x => x.FindAsync(jwtId))
                .ReturnsAsync((RefreshToken)null);

            await _repository.RemoveJwtIdAsync(jwtId);

            _refreshTokens.Verify(
                x => x.Remove(It.IsAny<RefreshToken>()),
                Times.Never);
        }

        [Test]
        public async Task SaveChangesAsync_CallsContextSaveChanges()
        {
            _context
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            await _repository.SaveChangesAsync();

            _context.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }
    }
}
