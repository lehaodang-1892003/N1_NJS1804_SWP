﻿using Microsoft.EntityFrameworkCore;
using ODTLearning.Entities;
using ODTLearning.Models;




namespace ODTLearning.Repositories
{
    public class ModeratorRepository : IModeratorRepository
    {

        private readonly DbminiCapstoneContext _context;

        public ModeratorRepository(DbminiCapstoneContext context)
        {
            _context = context;
        }

        public async Task<List<ListTutorToConfirm>?> GetListTutorsToCofirm()
        {
            var tutors = await _context.Tutors
                .Include(t => t.IdAccountNavigation)
                .Where(t => t.Status == "Operating")
                .Select(t => new ListTutorToConfirm
                {
                    Id = t.Id,
                    fullName = t.IdAccountNavigation.FullName
                })
                .ToListAsync();

            // Kiểm tra nếu danh sách tutor rỗng thì trả về null
            if (!tutors.Any())
            {
                return null;
            }

            return tutors;
        }
        public async Task<object?> GetTutorProfileToConfirm(string id)
        {
            var tutorDetails = await _context.Tutors
                .Include(t => t.IdAccountNavigation)
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.IdSubjectNavigation)
                .Include(t => t.EducationalQualifications)
                .Where(t => t.Id == id)
                .Select(t => new
                {
                    TutorId = t.Id,
                    SpecializedSkills = t.SpecializedSkills,
                    Experience = t.Experience,
                    Status = t.Status,
                    Account = new
                    {
                        Id = t.IdAccountNavigation.Id,
                        FullName = t.IdAccountNavigation.FullName,
                        Email = t.IdAccountNavigation.Email,
                        DateOfBirth = t.IdAccountNavigation.DateOfBirth,
                        Gender = t.IdAccountNavigation.Gender,
                        Roles = t.IdAccountNavigation.Roles,
                        Avatar = t.IdAccountNavigation.Avatar,
                        Address = t.IdAccountNavigation.Address,
                        Phone = t.IdAccountNavigation.Phone,
                        AccountBalance = t.IdAccountNavigation.AccountBalance
                    },
                    Fields = t.TutorSubjects.Select(ts => new
                    {
                        FieldId = ts.IdSubject,
                        FieldName = ts.IdSubjectNavigation.SubjectName
                    }),
                    EducationalQualifications = t.EducationalQualifications.Select(eq => new
                    {
                        CertificateName = eq.QualificationName,
                        Type = eq.Type,
                        Img = eq.Img
                    })
                })
                .FirstOrDefaultAsync();

            return tutorDetails;
        }
        public async Task<string> ChangeRequestLearningStatus(string requestId, string status)
        {
            // Tìm yêu cầu học tập theo IdRequest
            var request = await _context.Requests.FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
            {
                return "Request not found";
            }

            if (status.ToLower() == "approved")
            {
                request.Status = "approved";
                _context.Requests.Update(request);
                await _context.SaveChangesAsync();
                return "Request approved successfully";
            }
            else if (status.ToLower() == "reject")
            {
                return "Request rejected, no changes saved";
            }
            else
            {
                return "Invalid status provided";
            }
        }
        public async Task<bool> ConfirmProfileTutor(string idTutor, string status)
        {
            var tutor = await _context.Tutors.FirstOrDefaultAsync(x => x.Id == idTutor);
            if (tutor == null)
            {
                return false;
            }

            if (status.ToLower() == "approved" || status.ToLower() == "reject")
            {
                tutor.Status = status.ToLower();
                _context.Tutors.Update(tutor);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

    }
}
