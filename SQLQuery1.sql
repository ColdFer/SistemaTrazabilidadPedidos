SELECT
    u.Id,
    u.FirstName,
    u.LastName,
    u.Email,
    r.Name AS Role,
    u.IsActive
FROM Users u
INNER JOIN Roles r
    ON r.Id = u.RoleId
WHERE u.Email = 'admin@tecnoexpress.com';