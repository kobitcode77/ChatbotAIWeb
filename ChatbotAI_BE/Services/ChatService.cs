using ChatbotAI_BE.Data;
using ChatbotAI_BE.Dtos;
using ChatbotAI_BE.Exceptions;
using ChatbotAI_BE.Models;
using ChatbotAI_BE.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ChatbotAI_BE.Services
{
    public interface IChatService
    {
        Task<List<ChatSession>> GetSessionsAsync(Guid userId);
        Task<List<ChatMessage>> GetMessagesAsync(Guid sessionId, Guid userId);
        Task<ChatSession> CreateSessionAsync(Guid userId, CreateSessionRequest request);
        Task<bool> DeleteSessionAsync(Guid sessionId, Guid userId);
        Task<bool> DeleteMessageAsync(Guid messageId, Guid userId);
    }
    public class ChatService : IChatService
    {
        private readonly ISessionRepository _sessionRepo;
        private readonly IMessageRepository _messageRepo;

        public ChatService(ISessionRepository sessionRepo, IMessageRepository messageRepo)
        {
            _sessionRepo = sessionRepo;
            _messageRepo = messageRepo;
        }
        public async Task<List<ChatSession>> GetSessionsAsync(Guid userId)
        {
            try
            {
                return await _sessionRepo.GetSessionsAsync(userId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<ChatMessage>> GetMessagesAsync(Guid sessionId, Guid userId)
        {
            try
            {
                var messages = await _messageRepo.GetMessagesAsync(sessionId, userId);

                if (!messages.Any())
                    throw new SessionNotFoundException();

                return messages;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ChatSession> CreateSessionAsync(Guid userId, CreateSessionRequest request)
        {
            try
            {
                var session = new ChatSession
                {
                    UserId = userId,
                    Model = request.Model,
                    Title = string.IsNullOrWhiteSpace(request.Title)
                               ? $"{request.Model}"
                               : request.Title
                };

                await _sessionRepo.AddSessionAsync(session);
                await _sessionRepo.SaveChangesAsync();

                return session;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteSessionAsync(Guid sessionId, Guid userId)
        {
            try
            {
                var session = await _sessionRepo.GetSessionAsync(sessionId, userId);
                if (session == null)
                    throw new SessionNotFoundException();

                await _sessionRepo.DeleteSessionAsync(session);
                await _sessionRepo.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteMessageAsync(Guid messageId, Guid userId)
        {
            try
            {
                var message = await _messageRepo.GetMessageAsync(messageId, userId);
                if (message == null)
                    throw new MessageNotFoundException();

                await _messageRepo.DeleteMessageAsync(message);
                await _messageRepo.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) {
                throw;
            }
        }
    }
}
