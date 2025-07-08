<%@ Page Title="Expert Webinar Sessions" Language="C#" MasterPageFile="User.Master" AutoEventWireup="true" CodeBehind="ExpertWebinarHomepage.aspx.cs" Inherits="SpotTheScam.User.ExpertWebinarHomepage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .hero-section {
            background: linear-gradient(135deg, var(--brand-orange) 0%, #e67e22 100%);
            color: white;
            padding: 60px 0;
            margin-bottom: 50px;
            width: 100vw;
            position: relative;
            left: 50%;
            right: 50%;
            margin-left: -50vw;
            margin-right: -50vw;
        }

        .hero-icon {
            width: 120px;
            height: 120px;
            background-color: rgba(255, 255, 255, 0.9);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 60px;
            color: var(--brand-orange);
        }

        .hero-title {
            font-size: 2.5rem;
            font-weight: 700;
            margin-bottom: 20px;
        }

        .hero-description {
            font-size: 1.1rem;
            line-height: 1.6;
            opacity: 0.95;
        }

        .section-title {
            font-size: 1.8rem;
            font-weight: 700;
            color: var(--brand-navy);
            margin-bottom: 40px;
        }

        .expert-card {
            background: white;
            border-radius: 15px;
            padding: 30px;
            box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            margin-bottom: 30px;
            border: 2px solid transparent;
        }

        .expert-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.15);
            border-color: var(--brand-orange);
        }

        .expert-image {
            width: 80px;
            height: 80px;
            border-radius: 50%;
            object-fit: cover;
            margin-bottom: 20px;
        }

        .expert-name {
            font-size: 1.2rem;
            font-weight: 700;
            color: var(--brand-navy);
            margin-bottom: 5px;
        }

        .expert-title {
            font-size: 1rem;
            font-weight: 600;
            color: var(--brand-orange);
            margin-bottom: 10px;
        }

        .expert-description {
            font-size: 0.9rem;
            color: #666;
            margin-bottom: 15px;
            line-height: 1.5;
        }

        .specialty-tag {
            background-color: #FFC107;
            color: #000;
            padding: 6px 12px;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 600;
            display: inline-block;
        }

        .upcoming-section {
            background: linear-gradient(135deg, #FFF8E1 0%, #FFE0B2 100%);
            padding: 50px 0;
            margin: 50px 0;
            width: 100vw;
            position: relative;
            left: 50%;
            right: 50%;
            margin-left: -50vw;
            margin-right: -50vw;
        }

        .webinar-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
            margin-bottom: 20px;
            transition: transform 0.3s ease;
        }

        .webinar-card:hover {
            transform: translateY(-3px);
        }

        .webinar-date {
            background-color: var(--brand-orange);
            color: white;
            padding: 8px 15px;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 600;
            margin-bottom: 15px;
            display: inline-block;
        }

        .webinar-title {
            font-size: 1.1rem;
            font-weight: 700;
            color: var(--brand-navy);
            margin-bottom: 10px;
        }

        .webinar-description {
            font-size: 0.9rem;
            color: #666;
            margin-bottom: 20px;
            line-height: 1.5;
        }

        .register-btn {
            background-color: var(--brand-orange);
            color: white;
            border: none;
            padding: 10px 25px;
            border-radius: 25px;
            font-weight: 600;
            transition: background-color 0.3s ease;
            text-decoration: none;
            display: inline-block;
        }

        .register-btn:hover {
            background-color: #b45a22;
            color: white;
        }

        .how-it-works {
            margin: 60px 0;
        }

        .step-card {
            text-align: center;
            padding: 30px 20px;
        }

        .step-number {
            width: 60px;
            height: 60px;
            background-color: #FFC107;
            color: #000;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.5rem;
            font-weight: 700;
            margin: 0 auto 20px;
        }

        .step-title {
            font-size: 1.2rem;
            font-weight: 700;
            color: var(--brand-navy);
            margin-bottom: 15px;
        }

        .step-description {
            font-size: 0.9rem;
            color: #666;
            line-height: 1.5;
        }

        .view-schedule-btn {
            background-color: var(--brand-orange);
            color: white;
            border: none;
            padding: 15px 30px;
            border-radius: 25px;
            font-weight: 600;
            font-size: 1rem;
            margin-top: 30px;
            transition: background-color 0.3s ease;
        }

        .view-schedule-btn:hover {
            background-color: #b45a22;
        }

        .webinar-icon {
            width: 60px;
            height: 60px;
            background-color: rgba(211, 111, 45, 0.1);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 20px;
            font-size: 24px;
            color: var(--brand-orange);
        }

        @media (max-width: 768px) {
            .hero-title {
                font-size: 2rem;
            }
            
            .hero-description {
                font-size: 1rem;
            }
            
            .expert-card {
                padding: 20px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Hero Section -->
    <div class="hero-section">
        <div class="container">
            <div class="row align-items-center">
                <div class="col-lg-2 col-md-3 text-center">
                </div>
                <div class="col-lg-10 col-md-9">
                    <h1 class="hero-title">Webinar Sessions with Scam Experts</h1>
                    <p class="hero-description">
                        Want to learn more about the different ways to protect and prevent yourself from being a scam victim?
                        Keen on listening to industry experts about various methods commonly used by scammers?
                        Come join us for webinar sessions with experts to learn more about scams and have all your questions answered!
                    </p>
                </div>
            </div>
        </div>
    </div>

    <!-- Industry Experts Section -->
    <div class="container">
        <h2 class="section-title">Industry Experts</h2>
        <div class="row">
            <div class="col-lg-4 col-md-6 mb-4">
                <div class="expert-card">
                    <img src="/Images/expert1.jpg" alt="Maria Rodriguez" class="expert-image" />
                    <div class="expert-name">Maria Rodriguez</div>
                    <div class="expert-title">Digital Safety Educator</div>
                    <div class="expert-description">
                        Teaching seniors safe internet practices and social media awareness since 2015
                    </div>
                    <div class="specialty-tag">Specializes in: Social media & Online shopping</div>
                </div>
            </div>
            
            <div class="col-lg-4 col-md-6 mb-4">
                <div class="expert-card">
                    <img src="/Images/expert2.jpg" alt="Dr Harvey Blue" class="expert-image" />
                    <div class="expert-name">Dr Harvey Blue</div>
                    <div class="expert-title">Cybersecurity Specialist</div>
                    <div class="expert-description">
                        15+ years experience protecting seniors from online fraud and digital scams
                    </div>
                    <div class="specialty-tag">Specializes in: Banking security, Email scams</div>
                </div>
            </div>
            
            <div class="col-lg-4 col-md-6 mb-4">
                <div class="expert-card">
                    <img src="/Images/expert3.jpg" alt="Officer James Wilson" class="expert-image" />
                    <div class="expert-name">Officer James Wilson</div>
                    <div class="expert-title">Police Fraud Division</div>
                    <div class="expert-description">
                        Investigating phone and romance scams for 10+ years, helping victims recover
                    </div>
                    <div class="specialty-tag">Specializes in: Phone scams, Recovery assistance</div>
                </div>
            </div>
        </div>
    </div>

    <!-- Upcoming Sessions Section -->
    <div class="upcoming-section">
        <div class="container">
            <h2 class="section-title text-center">Upcoming Sessions This Week</h2>
            <div class="row">
                <div class="col-lg-4 col-md-6 mb-4">
                    <div class="webinar-card">
                        <div class="webinar-date">June 15, 2024 PM</div>
                        <div class="webinar-title">Protecting Your Online Banking</div>
                        <div class="webinar-description">
                            Learn secure banking practices and how to spot fraudulent banking websites with cybersecurity expert Dr. Harvey Blue
                        </div>
                        <a href="#" class="register-btn">Register Now</a>
                    </div>
                </div>
                
                <div class="col-lg-4 col-md-6 mb-4">
                    <div class="webinar-card">
                        <div class="webinar-date">June 17, 10:00 AM</div>
                        <div class="webinar-title">Latest Phone Scam Tactics</div>
                        <div class="webinar-description">
                            Discover the newest phone scam methods and how to protect yourself with Officer James Wilson from the Police Fraud Division
                        </div>
                        <a href="#" class="register-btn">Register Now</a>
                    </div>
                </div>
                
                <div class="col-lg-4 col-md-6 mb-4">
                    <div class="webinar-card">
                        <div class="webinar-date">June 19, 2:00 PM</div>
                        <div class="webinar-title">Safe Social Media for Seniors</div>
                        <div class="webinar-description">
                            Navigate Facebook, Instagram, and other platforms safely while avoiding scammers with digital educator Maria Rodriguez
                        </div>
                        <a href="#" class="register-btn">Register Now</a>
                    </div>
                </div>
            </div>
            
            <div class="text-center">
                <button class="view-schedule-btn">View Full Schedule →</button>
            </div>
        </div>
    </div>

    <!-- How Our Webinars Work Section -->
    <div class="how-it-works">
        <div class="container">
            <h2 class="section-title text-center">How Our Webinars Work</h2>
            <div class="row">
                <div class="col-lg-4 col-md-6 mb-4">
                    <div class="step-card">
                        <div class="step-number">1</div>
                        <div class="step-title">Choose a Session</div>
                        <div class="step-description">
                            Browse topics that interest you from our schedule of expert-led sessions
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-4 col-md-6 mb-4">
                    <div class="step-card">
                        <div class="step-number">2</div>
                        <div class="step-title">Register for Free</div>
                        <div class="step-description">
                            All sessions are free for registered users. Just click register and you're set!
                        </div>
                    </div>
                </div>
                
                <div class="col-lg-4 col-md-6 mb-4">
                    <div class="step-card">
                        <div class="step-number">3</div>
                        <div class="step-title">Join Live</div>
                        <div class="step-description">
                            Ask questions and interact directly with cybersecurity experts and police officers
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>