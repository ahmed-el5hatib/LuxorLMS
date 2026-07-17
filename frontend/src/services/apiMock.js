// LuxorLMS State Engine & API Bridge for Milestones 1 - 6

export const INITIAL_USER = {
  id: "u-9910-student",
  username: "ahmed.elkhatib",
  fullName: "Ahmed El-khatib",
  email: "a.elkhatib@luxor.edu.eg",
  role: "Student", // "Student" | "Doctor" | "TA" | "Admin"
  avatar: "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&q=80&w=150",
  gpa: 3.85,
  completedCredits: 84,
  currentSemesterCredits: 15,
  attendancePercentage: 96.2,
};

export const INITIAL_COURSES = [
  {
    id: "offering-101",
    courseCode: "CS304",
    nameEn: "Distributed Systems & Cloud Architecture",
    nameAr: "الأنظمة الموزعة وهندسة السحاب",
    department: "Computer Science",
    creditHours: 3,
    primaryTeacher: "Dr. Hassan Mahmoud",
    capacity: 60,
    enrolledCount: 42,
    isEnrolled: true,
    sectionType: "Lecture + Lab",
    status: "Published",
  },
  {
    id: "offering-102",
    courseCode: "CS401",
    nameEn: "Machine Learning & Neural Networks",
    nameAr: "التعلم الآلي والشبكات العصبية",
    department: "Artificial Intelligence",
    creditHours: 4,
    primaryTeacher: "Dr. Layla Mansour",
    capacity: 55,
    enrolledCount: 51,
    isEnrolled: true,
    sectionType: "Lecture + Project",
    status: "Published",
  },
  {
    id: "offering-103",
    courseCode: "IS302",
    nameEn: "Enterprise Relational Databases & NoSQL",
    nameAr: "قواعد البيانات المؤسسية",
    department: "Information Systems",
    creditHours: 3,
    primaryTeacher: "Dr. Sherif Zaki",
    capacity: 70,
    enrolledCount: 38,
    isEnrolled: false,
    sectionType: "Lecture + Lab",
    status: "Published",
  },
  {
    id: "offering-104",
    courseCode: "SE205",
    nameEn: "Modern Software Engineering & Design Patterns",
    nameAr: "هندسة البرمجيات والتصميم",
    department: "Software Engineering",
    creditHours: 3,
    primaryTeacher: "Dr. Ahmed Refaat",
    capacity: 50,
    enrolledCount: 45,
    isEnrolled: true,
    sectionType: "Lecture",
    status: "Published",
  }
];

export const INITIAL_GRADES = [
  { courseCode: "CS304", title: "Distributed Systems", creditHours: 3, grade: "A", score: 94, semester: "Spring 2026" },
  { courseCode: "CS401", title: "Machine Learning", creditHours: 4, grade: "A-", score: 89, semester: "Spring 2026" },
  { courseCode: "SE205", title: "Software Engineering", creditHours: 3, grade: "A+", score: 98, semester: "Spring 2026" },
  { courseCode: "MATH201", title: "Linear Algebra & Calculus", creditHours: 3, grade: "B+", score: 87, semester: "Fall 2025" },
  { courseCode: "CS201", title: "Data Structures & Algorithms", creditHours: 4, grade: "A", score: 95, semester: "Fall 2025" },
];

export const INITIAL_QUIZZES = [
  {
    id: "q-101",
    title: "Midterm Exam: Distributed Consensus (Raft/Paxos)",
    courseCode: "CS304",
    durationMinutes: 30,
    totalQuestions: 5,
    dueDate: "2026-07-20T23:59:00",
    status: "Pending",
    questions: [
      {
        id: 1,
        questionText: "Which consensus algorithm uses a Leader-Follower-Candidate state machine representation?",
        options: ["Raft", "Paxos", "Two-Phase Commit", "Byzantine Fault Tolerance"],
        correctIndex: 0
      },
      {
        id: 2,
        questionText: "What is the primary goal of partitioning in relational multi-tenant databases?",
        options: ["Improves network speed", "Scales reads/writes across tables & CPU nodes", "Removes primary keys", "Enforces ACID"],
        correctIndex: 1
      }
    ]
  },
  {
    id: "q-102",
    title: "Quiz 2: Deep Convolutional Neural Networks",
    courseCode: "CS401",
    durationMinutes: 15,
    totalQuestions: 3,
    dueDate: "2026-07-22T18:00:00",
    status: "Completed",
    score: "100%"
  }
];

export const INITIAL_FILES = [
  {
    id: "f-1001",
    fileName: "CS304_Project_Milestone_6_Spec.pdf",
    contentType: "application/pdf",
    sizeBytes: 2450000,
    version: 3,
    provider: "S3",
    container: "luxorlms-submissions",
    createdAt: "2026-07-16T22:10:00Z",
    versions: [
      { version: 1, objectKey: "f1001_v1.pdf", createdAt: "2026-07-10T10:00:00Z", sizeBytes: 2100000 },
      { version: 2, objectKey: "f1001_v2.pdf", createdAt: "2026-07-14T14:30:00Z", sizeBytes: 2350000 },
      { version: 3, objectKey: "f1001_v3.pdf", createdAt: "2026-07-16T22:10:00Z", sizeBytes: 2450000 },
    ]
  },
  {
    id: "f-1002",
    fileName: "CS401_Lab_Assignment_03.zip",
    contentType: "application/zip",
    sizeBytes: 15800000,
    version: 1,
    provider: "MinIO",
    container: "luxorlms-course-materials",
    createdAt: "2026-07-15T09:15:00Z",
    versions: [
      { version: 1, objectKey: "f1002_v1.zip", createdAt: "2026-07-15T09:15:00Z", sizeBytes: 15800000 }
    ]
  }
];

export const INITIAL_NOTIFICATIONS = [
  {
    id: "n-1",
    title: "Registration Open for Fall 2026",
    body: "Academic registration window for Fall 2026 is now officially open until July 30.",
    channel: "InApp",
    status: "Sent",
    createdAt: "10 mins ago"
  },
  {
    id: "n-2",
    title: "Grade Published: CS304 Midterm",
    body: "Your grade for CS304 Midterm Exam has been published. Score: 94/100 (A).",
    channel: "Email",
    status: "Sent",
    createdAt: "1 hour ago"
  },
  {
    id: "n-3",
    title: "Attendance Warning: CS401",
    body: "Your attendance rate for CS401 is 88%. Keep above 85% to avoid exam disqualification.",
    channel: "Sms",
    status: "Sent",
    createdAt: "Yesterday"
  }
];

export const INITIAL_FORUMS = [
  {
    id: "topic-1",
    courseOfferingId: "offering-101",
    courseCode: "CS304",
    title: "Discussion: Optimizing Hangfire PostgreSQL Background Job Dispatchers",
    authorName: "Dr. Hassan Mahmoud",
    isDoctor: true,
    isPinned: true,
    isLocked: false,
    createdAt: "2026-07-16T18:00:00Z",
    repliesCount: 4,
    posts: [
      {
        id: "post-1",
        authorName: "Dr. Hassan Mahmoud",
        isDoctor: true,
        body: "Hello students, for Milestone 6 please ensure your background jobs handle retry exponential backoffs without blocking HTTP response loops.",
        createdAt: "2026-07-16T18:00:00Z",
        moderationStatus: "None"
      },
      {
        id: "post-2",
        authorName: "Ahmed El-khatib",
        isDoctor: false,
        body: "Thank you Dr. Hassan! Should we configure isolated DbContext instances for background job runners?",
        createdAt: "2026-07-16T18:45:00Z",
        moderationStatus: "None"
      }
    ]
  },
  {
    id: "topic-2",
    courseOfferingId: "offering-102",
    courseCode: "CS401",
    title: "PyTorch vs TensorFlow for Convolutional Layers Benchmark",
    authorName: "Eng. Omar Khaled (TA)",
    isDoctor: false,
    isPinned: false,
    isLocked: false,
    createdAt: "2026-07-15T12:30:00Z",
    repliesCount: 2,
    posts: [
      {
        id: "post-3",
        authorName: "Eng. Omar Khaled (TA)",
        isDoctor: false,
        body: "Please share your speed benchmarks for PyTorch 2.3 vs TF 2.16 on CUDA 12.4.",
        createdAt: "2026-07-15T12:30:00Z",
        moderationStatus: "None"
      }
    ]
  }
];
