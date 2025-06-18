# Pull Request Template

## 🎫 **Jira Ticket Information**
**Ticket:** [HE-XXX](https://unosquare.atlassian.net/browse/HE-XXX)  
**Title:** <!-- Insert ticket title -->  
**Priority:** <!-- High/Medium/Low -->  
**Type:** <!-- Bug/Feature/Task/Story -->

---

## 📋 **Acceptance Criteria**
<!-- Copy acceptance criteria from Jira ticket and check off as completed -->

- [ ] **AC1:** <!-- Description of acceptance criteria 1 -->
- [ ] **AC2:** <!-- Description of acceptance criteria 2 -->
- [ ] **AC3:** <!-- Description of acceptance criteria 3 -->

---

## 📝 **Summary of Changes**

### **What was implemented:**
<!-- Brief description of what this PR accomplishes -->

### **Files Modified:**
<!-- List of files that were changed -->
- `src/api/Controllers/` - <!-- Description of changes -->
- `src/api/ViewModels/` - <!-- Description of changes -->
- `test/` - <!-- Description of test changes -->

### **Key Changes:**
<!-- Detailed list of the main changes made -->
1. **Added:** <!-- New functionality/features -->
2. **Modified:** <!-- Changed existing functionality -->
3. **Fixed:** <!-- Bug fixes -->
4. **Removed:** <!-- Deprecated/removed functionality -->

---

## 🧪 **Testing**

### **Manual Testing Performed:**
- [ ] **Happy Path:** <!-- Description of successful scenario testing -->
- [ ] **Edge Cases:** <!-- Description of edge case testing -->
- [ ] **Error Handling:** <!-- Description of error scenario testing -->

### **Automated Tests:**
- [ ] **Unit Tests:** Added/Updated for new functionality
- [ ] **Integration Tests:** Added/Updated for API endpoints
- [ ] **All Tests Passing:** Confirmed all existing tests still pass

### **API Testing:**
<!-- If API changes, include example requests/responses -->
```bash
# Example API calls demonstrating the changes
curl -X POST /api/endpoint
curl -X GET /api/endpoint/{id}
curl -X DELETE /api/endpoint/{id}
```

---

## 🏗️ **Technical Implementation**

### **Architecture Decisions:**
<!-- Any significant architectural or design decisions made -->

### **Dependencies:**
<!-- New dependencies added or removed -->
- **Added:** <!-- List new packages/dependencies -->
- **Updated:** <!-- List updated packages -->

### **Database Changes:**
<!-- Any database schema or data changes -->
- [ ] **Schema Changes:** None / <!-- Description -->
- [ ] **Migration Required:** No / <!-- Migration details -->

---

## 🔍 **Code Review Checklist**

### **Code Quality:**
- [ ] **Follows C# Coding Standards** (PascalCase, proper naming conventions)
- [ ] **Error Handling** implemented with proper HTTP status codes
- [ ] **Input Validation** added where necessary
- [ ] **No Code Duplication** or violations of DRY principle
- [ ] **Performance Considerations** addressed

### **Security:**
- [ ] **Input Sanitization** properly implemented
- [ ] **Authorization** checks in place (if applicable)
- [ ] **No Sensitive Data** exposed in logs or responses

### **API Design:**
- [ ] **RESTful Conventions** followed
- [ ] **Consistent Response Format** using ViewModels
- [ ] **Proper HTTP Status Codes** (200, 201, 400, 404, 500, etc.)
- [ ] **API Documentation** updated (if applicable)

---

## 🚀 **Deployment Notes**

### **Pre-deployment:**
- [ ] **Configuration Changes:** None required / <!-- Details -->
- [ ] **Environment Variables:** None required / <!-- Details -->

### **Post-deployment:**
- [ ] **Monitoring:** <!-- Any specific monitoring to watch -->
- [ ] **Rollback Plan:** <!-- Steps to rollback if issues arise -->

---

## 🔗 **Related Links**
- **Jira Ticket:** [HE-XXX](https://unosquare.atlassian.net/browse/HE-XXX)
- **Design Documents:** <!-- Link to any design docs -->
- **Related PRs:** <!-- Link to related pull requests -->

---

## 📸 **Screenshots/Evidence**
<!-- Include screenshots of new UI, API responses, or test results -->

---

## 👥 **Reviewers**
<!-- Tag specific reviewers if needed -->
@<!-- username --> - <!-- reason for review -->

---

**Ready for Review:** ✅ / ⏳  
**Ready for Merge:** ✅ / ⏳ 