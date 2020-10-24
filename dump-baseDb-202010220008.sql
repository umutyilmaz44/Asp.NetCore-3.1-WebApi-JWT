--
-- PostgreSQL database dump
--

-- Dumped from database version 9.2.24
-- Dumped by pg_dump version 12.2

-- Started on 2020-10-22 00:08:19

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 6 (class 2615 OID 2200)
-- Name: public; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA public;


ALTER SCHEMA public OWNER TO postgres;

--
-- TOC entry 2987 (class 0 OID 0)
-- Dependencies: 6
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: postgres
--

COMMENT ON SCHEMA public IS 'standard public schema';


--
-- TOC entry 169 (class 1259 OID 297056)
-- Name: grand_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.grand_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.grand_id_seq OWNER TO postgres;

SET default_tablespace = '';

--
-- TOC entry 170 (class 1259 OID 297060)
-- Name: grand; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.grand (
    id bigint DEFAULT nextval('public.grand_id_seq'::regclass) NOT NULL,
    grand_name character varying NOT NULL
);


ALTER TABLE public.grand OWNER TO postgres;

--
-- TOC entry 184 (class 1259 OID 297191)
-- Name: grand_role; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.grand_role (
    id bigint NOT NULL,
    role_id bigint NOT NULL,
    grand_id bigint NOT NULL
);


ALTER TABLE public.grand_role OWNER TO postgres;

--
-- TOC entry 183 (class 1259 OID 297189)
-- Name: grand_role_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.grand_role_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.grand_role_id_seq OWNER TO postgres;

--
-- TOC entry 2989 (class 0 OID 0)
-- Dependencies: 183
-- Name: grand_role_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.grand_role_id_seq OWNED BY public.grand_role.id;


--
-- TOC entry 172 (class 1259 OID 297076)
-- Name: organization; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.organization (
    id bigint NOT NULL,
    parent_id bigint,
    title text NOT NULL,
    description text,
    record_date timestamp without time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.organization OWNER TO postgres;

--
-- TOC entry 171 (class 1259 OID 297074)
-- Name: organization_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.organization_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.organization_id_seq OWNER TO postgres;

--
-- TOC entry 2990 (class 0 OID 0)
-- Dependencies: 171
-- Name: organization_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.organization_id_seq OWNED BY public.organization.id;


--
-- TOC entry 186 (class 1259 OID 297218)
-- Name: page; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.page (
    id bigint NOT NULL,
    page_name character varying NOT NULL,
    naviagte_url character varying NOT NULL
);


ALTER TABLE public.page OWNER TO postgres;

--
-- TOC entry 185 (class 1259 OID 297216)
-- Name: page_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.page_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.page_id_seq OWNER TO postgres;

--
-- TOC entry 2991 (class 0 OID 0)
-- Dependencies: 185
-- Name: page_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.page_id_seq OWNED BY public.page.id;


--
-- TOC entry 188 (class 1259 OID 297231)
-- Name: refresh_token; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.refresh_token (
    id bigint NOT NULL,
    user_id bigint NOT NULL,
    token character varying NOT NULL,
    expiry_date timestamp(0) without time zone NOT NULL
);


ALTER TABLE public.refresh_token OWNER TO postgres;

--
-- TOC entry 187 (class 1259 OID 297229)
-- Name: refresh_token_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.refresh_token_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.refresh_token_id_seq OWNER TO postgres;

--
-- TOC entry 2992 (class 0 OID 0)
-- Dependencies: 187
-- Name: refresh_token_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.refresh_token_id_seq OWNED BY public.refresh_token.id;


--
-- TOC entry 180 (class 1259 OID 297155)
-- Name: role; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.role (
    id bigint NOT NULL,
    role_name character varying NOT NULL,
    "desc" character varying
);


ALTER TABLE public.role OWNER TO postgres;

--
-- TOC entry 179 (class 1259 OID 297153)
-- Name: role_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.role_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.role_id_seq OWNER TO postgres;

--
-- TOC entry 2993 (class 0 OID 0)
-- Dependencies: 179
-- Name: role_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.role_id_seq OWNED BY public.role.id;


--
-- TOC entry 176 (class 1259 OID 297110)
-- Name: user; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."user" (
    id bigint NOT NULL,
    first_name character varying NOT NULL,
    middle_name character varying,
    last_name character varying,
    email_address character varying,
    password character varying NOT NULL,
    organization_id bigint DEFAULT 1 NOT NULL,
    last_login_time timestamp(0) without time zone,
    user_type_id bigint DEFAULT 1 NOT NULL,
    deleted boolean DEFAULT false NOT NULL
);


ALTER TABLE public."user" OWNER TO postgres;

--
-- TOC entry 175 (class 1259 OID 297108)
-- Name: user_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.user_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.user_id_seq OWNER TO postgres;

--
-- TOC entry 2994 (class 0 OID 0)
-- Dependencies: 175
-- Name: user_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.user_id_seq OWNED BY public."user".id;


--
-- TOC entry 178 (class 1259 OID 297139)
-- Name: user_login; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.user_login (
    id bigint NOT NULL,
    user_id bigint NOT NULL,
    login_time timestamp(0) without time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.user_login OWNER TO postgres;

--
-- TOC entry 177 (class 1259 OID 297137)
-- Name: user_login_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.user_login_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.user_login_id_seq OWNER TO postgres;

--
-- TOC entry 2995 (class 0 OID 0)
-- Dependencies: 177
-- Name: user_login_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.user_login_id_seq OWNED BY public.user_login.id;


--
-- TOC entry 182 (class 1259 OID 297169)
-- Name: user_role; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.user_role (
    id bigint NOT NULL,
    user_id bigint NOT NULL,
    role_id bigint NOT NULL
);


ALTER TABLE public.user_role OWNER TO postgres;

--
-- TOC entry 181 (class 1259 OID 297167)
-- Name: user_role_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.user_role_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.user_role_id_seq OWNER TO postgres;

--
-- TOC entry 2996 (class 0 OID 0)
-- Dependencies: 181
-- Name: user_role_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.user_role_id_seq OWNED BY public.user_role.id;


--
-- TOC entry 174 (class 1259 OID 297092)
-- Name: user_type; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.user_type (
    id bigint NOT NULL,
    type_name character varying NOT NULL,
    type_description character varying,
    token_life_time integer DEFAULT 60 NOT NULL
);


ALTER TABLE public.user_type OWNER TO postgres;

--
-- TOC entry 2997 (class 0 OID 0)
-- Dependencies: 174
-- Name: COLUMN user_type.token_life_time; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.user_type.token_life_time IS 'second based time interval';


--
-- TOC entry 173 (class 1259 OID 297090)
-- Name: user_type_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.user_type_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.user_type_id_seq OWNER TO postgres;

--
-- TOC entry 2998 (class 0 OID 0)
-- Dependencies: 173
-- Name: user_type_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.user_type_id_seq OWNED BY public.user_type.id;


--
-- TOC entry 2806 (class 2604 OID 297194)
-- Name: grand_role id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grand_role ALTER COLUMN id SET DEFAULT nextval('public.grand_role_id_seq'::regclass);


--
-- TOC entry 2794 (class 2604 OID 297079)
-- Name: organization id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.organization ALTER COLUMN id SET DEFAULT nextval('public.organization_id_seq'::regclass);


--
-- TOC entry 2807 (class 2604 OID 297221)
-- Name: page id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.page ALTER COLUMN id SET DEFAULT nextval('public.page_id_seq'::regclass);


--
-- TOC entry 2808 (class 2604 OID 297234)
-- Name: refresh_token id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.refresh_token ALTER COLUMN id SET DEFAULT nextval('public.refresh_token_id_seq'::regclass);


--
-- TOC entry 2804 (class 2604 OID 297158)
-- Name: role id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.role ALTER COLUMN id SET DEFAULT nextval('public.role_id_seq'::regclass);


--
-- TOC entry 2798 (class 2604 OID 297113)
-- Name: user id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."user" ALTER COLUMN id SET DEFAULT nextval('public.user_id_seq'::regclass);


--
-- TOC entry 2802 (class 2604 OID 297142)
-- Name: user_login id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_login ALTER COLUMN id SET DEFAULT nextval('public.user_login_id_seq'::regclass);


--
-- TOC entry 2805 (class 2604 OID 297172)
-- Name: user_role id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_role ALTER COLUMN id SET DEFAULT nextval('public.user_role_id_seq'::regclass);


--
-- TOC entry 2796 (class 2604 OID 297095)
-- Name: user_type id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_type ALTER COLUMN id SET DEFAULT nextval('public.user_type_id_seq'::regclass);


--
-- TOC entry 2963 (class 0 OID 297060)
-- Dependencies: 170
-- Data for Name: grand; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.grand VALUES (1, 'Organization-Create');
INSERT INTO public.grand VALUES (2, 'Organization-Delete');
INSERT INTO public.grand VALUES (3, 'Organization-Read');
INSERT INTO public.grand VALUES (4, 'Organization-Update');
INSERT INTO public.grand VALUES (5, 'User-Create');
INSERT INTO public.grand VALUES (6, 'User-Delete');
INSERT INTO public.grand VALUES (7, 'User-Read');
INSERT INTO public.grand VALUES (8, 'User-Update');
INSERT INTO public.grand VALUES (9, 'Role-Create');
INSERT INTO public.grand VALUES (10, 'Role-Delete');
INSERT INTO public.grand VALUES (11, 'Role-Read');
INSERT INTO public.grand VALUES (12, 'Role-Update');
INSERT INTO public.grand VALUES (13, 'UserLogin-Create');
INSERT INTO public.grand VALUES (14, 'UserLogin-Delete');
INSERT INTO public.grand VALUES (15, 'UserLogin-Read');
INSERT INTO public.grand VALUES (16, 'UserLogin-Update');
INSERT INTO public.grand VALUES (17, 'RefreshToken-Create');
INSERT INTO public.grand VALUES (18, 'RefreshToken-Delete');
INSERT INTO public.grand VALUES (19, 'RefreshToken-Read');
INSERT INTO public.grand VALUES (20, 'RefreshToken-Update');
INSERT INTO public.grand VALUES (21, 'Page-Create');
INSERT INTO public.grand VALUES (22, 'Page-Delete');
INSERT INTO public.grand VALUES (23, 'Page-Read');
INSERT INTO public.grand VALUES (24, 'Page-Update');


--
-- TOC entry 2977 (class 0 OID 297191)
-- Dependencies: 184
-- Data for Name: grand_role; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.grand_role VALUES (1, 1, 1);
INSERT INTO public.grand_role VALUES (2, 1, 2);
INSERT INTO public.grand_role VALUES (3, 1, 3);
INSERT INTO public.grand_role VALUES (4, 1, 4);
INSERT INTO public.grand_role VALUES (5, 1, 5);
INSERT INTO public.grand_role VALUES (6, 1, 6);
INSERT INTO public.grand_role VALUES (7, 1, 7);
INSERT INTO public.grand_role VALUES (8, 1, 8);
INSERT INTO public.grand_role VALUES (9, 1, 9);
INSERT INTO public.grand_role VALUES (10, 1, 10);
INSERT INTO public.grand_role VALUES (11, 1, 11);
INSERT INTO public.grand_role VALUES (12, 1, 12);
INSERT INTO public.grand_role VALUES (13, 1, 13);
INSERT INTO public.grand_role VALUES (14, 1, 14);
INSERT INTO public.grand_role VALUES (15, 1, 15);
INSERT INTO public.grand_role VALUES (16, 1, 16);
INSERT INTO public.grand_role VALUES (17, 1, 17);
INSERT INTO public.grand_role VALUES (18, 1, 18);
INSERT INTO public.grand_role VALUES (19, 1, 19);
INSERT INTO public.grand_role VALUES (20, 1, 20);
INSERT INTO public.grand_role VALUES (21, 1, 21);
INSERT INTO public.grand_role VALUES (22, 1, 22);
INSERT INTO public.grand_role VALUES (23, 1, 23);
INSERT INTO public.grand_role VALUES (24, 1, 24);


--
-- TOC entry 2965 (class 0 OID 297076)
-- Dependencies: 172
-- Data for Name: organization; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.organization VALUES (1, NULL, 'Root Company', 'This organization is root', '2020-10-20 11:55:10.526299');


--
-- TOC entry 2979 (class 0 OID 297218)
-- Dependencies: 186
-- Data for Name: page; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 2981 (class 0 OID 297231)
-- Dependencies: 188
-- Data for Name: refresh_token; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 2973 (class 0 OID 297155)
-- Dependencies: 180
-- Data for Name: role; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.role VALUES (1, 'Supervisor', 'This role for system admin');
INSERT INTO public.role VALUES (2, 'Administrator', 'This role for company admin');
INSERT INTO public.role VALUES (3, 'Personal', 'This role for company personal');


--
-- TOC entry 2969 (class 0 OID 297110)
-- Dependencies: 176
-- Data for Name: user; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public."user" VALUES (1, 'Mühüttün', NULL, 'Gandak', 'muhuttun.gandak@gmail.com', 'ABBkzVj5eYtwX8Xkjt+v4/TlT/dedcp2RVptL4JX/KRHBEQ7f+c0nutMv06zdUpELA==', 1, NULL, 1, false);


--
-- TOC entry 2971 (class 0 OID 297139)
-- Dependencies: 178
-- Data for Name: user_login; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 2975 (class 0 OID 297169)
-- Dependencies: 182
-- Data for Name: user_role; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.user_role VALUES (1, 1, 1);


--
-- TOC entry 2967 (class 0 OID 297092)
-- Dependencies: 174
-- Data for Name: user_type; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.user_type VALUES (1, 'WebAppUser', 'User using the system via web application', 3600);
INSERT INTO public.user_type VALUES (2, 'WebApiUser', 'User using the system via web api', 60);


--
-- TOC entry 2999 (class 0 OID 0)
-- Dependencies: 169
-- Name: grand_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.grand_id_seq', 24, true);


--
-- TOC entry 3000 (class 0 OID 0)
-- Dependencies: 183
-- Name: grand_role_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.grand_role_id_seq', 24, true);


--
-- TOC entry 3001 (class 0 OID 0)
-- Dependencies: 171
-- Name: organization_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.organization_id_seq', 1, true);


--
-- TOC entry 3002 (class 0 OID 0)
-- Dependencies: 185
-- Name: page_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.page_id_seq', 1, false);


--
-- TOC entry 3003 (class 0 OID 0)
-- Dependencies: 187
-- Name: refresh_token_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.refresh_token_id_seq', 1, true);


--
-- TOC entry 3004 (class 0 OID 0)
-- Dependencies: 179
-- Name: role_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.role_id_seq', 3, true);


--
-- TOC entry 3005 (class 0 OID 0)
-- Dependencies: 175
-- Name: user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_id_seq', 1, true);


--
-- TOC entry 3006 (class 0 OID 0)
-- Dependencies: 177
-- Name: user_login_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_login_id_seq', 1, true);


--
-- TOC entry 3007 (class 0 OID 0)
-- Dependencies: 181
-- Name: user_role_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_role_id_seq', 1, true);


--
-- TOC entry 3008 (class 0 OID 0)
-- Dependencies: 173
-- Name: user_type_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_type_id_seq', 1, false);


--
-- TOC entry 2811 (class 2606 OID 297068)
-- Name: grand grand_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grand
    ADD CONSTRAINT grand_pk PRIMARY KEY (id);


--
-- TOC entry 2837 (class 2606 OID 297196)
-- Name: grand_role grand_role_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grand_role
    ADD CONSTRAINT grand_role_pk PRIMARY KEY (id);


--
-- TOC entry 2814 (class 2606 OID 297085)
-- Name: organization organization_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.organization
    ADD CONSTRAINT organization_pk PRIMARY KEY (id);


--
-- TOC entry 2842 (class 2606 OID 297226)
-- Name: page pages_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.page
    ADD CONSTRAINT pages_pk PRIMARY KEY (id);


--
-- TOC entry 2846 (class 2606 OID 297239)
-- Name: refresh_token refreshtoken_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.refresh_token
    ADD CONSTRAINT refreshtoken_pk PRIMARY KEY (id);


--
-- TOC entry 2829 (class 2606 OID 297163)
-- Name: role role_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.role
    ADD CONSTRAINT role_pk PRIMARY KEY (id);


--
-- TOC entry 2826 (class 2606 OID 297145)
-- Name: user_login user_login_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_login
    ADD CONSTRAINT user_login_pk PRIMARY KEY (id);


--
-- TOC entry 2823 (class 2606 OID 297121)
-- Name: user user_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."user"
    ADD CONSTRAINT user_pk PRIMARY KEY (id);


--
-- TOC entry 2832 (class 2606 OID 297174)
-- Name: user_role user_role_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_role
    ADD CONSTRAINT user_role_pk PRIMARY KEY (id);


--
-- TOC entry 2817 (class 2606 OID 297101)
-- Name: user_type user_type_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_type
    ADD CONSTRAINT user_type_pk PRIMARY KEY (id);


--
-- TOC entry 2809 (class 1259 OID 297069)
-- Name: grand_grand_name_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX grand_grand_name_idx ON public.grand USING btree (grand_name);


--
-- TOC entry 2835 (class 1259 OID 297197)
-- Name: grand_role_grand_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX grand_role_grand_id_idx ON public.grand_role USING btree (grand_id);


--
-- TOC entry 2838 (class 1259 OID 297198)
-- Name: grand_role_role_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX grand_role_role_id_idx ON public.grand_role USING btree (role_id);


--
-- TOC entry 2812 (class 1259 OID 297086)
-- Name: organization_parentid_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX organization_parentid_idx ON public.organization USING btree (parent_id);


--
-- TOC entry 2815 (class 1259 OID 297087)
-- Name: organization_title_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX organization_title_idx ON public.organization USING btree (title);


--
-- TOC entry 2839 (class 1259 OID 297227)
-- Name: pages_naviagte_url_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX pages_naviagte_url_idx ON public.page USING btree (naviagte_url);


--
-- TOC entry 2840 (class 1259 OID 297228)
-- Name: pages_page_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX pages_page_idx ON public.page USING btree (page_name);


--
-- TOC entry 2843 (class 1259 OID 297240)
-- Name: refresh_token_token_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX refresh_token_token_idx ON public.refresh_token USING btree (token);


--
-- TOC entry 2844 (class 1259 OID 297241)
-- Name: refreshtoken_expirydate_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX refreshtoken_expirydate_idx ON public.refresh_token USING btree (expiry_date);


--
-- TOC entry 2847 (class 1259 OID 297242)
-- Name: refreshtoken_userid_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX refreshtoken_userid_idx ON public.refresh_token USING btree (user_id);


--
-- TOC entry 2830 (class 1259 OID 297164)
-- Name: role_rolename_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX role_rolename_idx ON public.role USING btree (role_name);


--
-- TOC entry 2819 (class 1259 OID 297122)
-- Name: user_email_address_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX user_email_address_idx ON public."user" USING btree (email_address);


--
-- TOC entry 2820 (class 1259 OID 297123)
-- Name: user_firstname_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX user_firstname_idx ON public."user" USING btree (first_name);


--
-- TOC entry 2824 (class 1259 OID 297146)
-- Name: user_login_login_time_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX user_login_login_time_idx ON public.user_login USING btree (login_time);


--
-- TOC entry 2827 (class 1259 OID 297147)
-- Name: user_login_user_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX user_login_user_id_idx ON public.user_login USING btree (user_id);


--
-- TOC entry 2821 (class 1259 OID 297124)
-- Name: user_organizationid_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX user_organizationid_idx ON public."user" USING btree (organization_id);


--
-- TOC entry 2833 (class 1259 OID 297175)
-- Name: user_role_role_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX user_role_role_id_idx ON public.user_role USING btree (role_id);


--
-- TOC entry 2834 (class 1259 OID 297176)
-- Name: user_role_user_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX user_role_user_id_idx ON public.user_role USING btree (user_id);


--
-- TOC entry 2818 (class 1259 OID 297102)
-- Name: user_type_type_name_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX user_type_type_name_idx ON public.user_type USING btree (type_name);


--
-- TOC entry 2853 (class 2606 OID 297199)
-- Name: grand_role grand_role_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grand_role
    ADD CONSTRAINT grand_role_fk FOREIGN KEY (grand_id) REFERENCES public.grand(id);


--
-- TOC entry 2854 (class 2606 OID 297204)
-- Name: grand_role grand_role_fk_1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grand_role
    ADD CONSTRAINT grand_role_fk_1 FOREIGN KEY (role_id) REFERENCES public.role(id);


--
-- TOC entry 2855 (class 2606 OID 297243)
-- Name: refresh_token refreshtoken_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.refresh_token
    ADD CONSTRAINT refreshtoken_fk FOREIGN KEY (user_id) REFERENCES public."user"(id);


--
-- TOC entry 2848 (class 2606 OID 297125)
-- Name: user user_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."user"
    ADD CONSTRAINT user_fk FOREIGN KEY (organization_id) REFERENCES public.organization(id);


--
-- TOC entry 2849 (class 2606 OID 297130)
-- Name: user user_fk_1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."user"
    ADD CONSTRAINT user_fk_1 FOREIGN KEY (user_type_id) REFERENCES public.user_type(id);


--
-- TOC entry 2850 (class 2606 OID 297148)
-- Name: user_login user_login_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_login
    ADD CONSTRAINT user_login_fk FOREIGN KEY (user_id) REFERENCES public."user"(id);


--
-- TOC entry 2851 (class 2606 OID 297177)
-- Name: user_role user_role_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_role
    ADD CONSTRAINT user_role_fk FOREIGN KEY (user_id) REFERENCES public."user"(id);


--
-- TOC entry 2852 (class 2606 OID 297182)
-- Name: user_role user_role_fk_1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_role
    ADD CONSTRAINT user_role_fk_1 FOREIGN KEY (role_id) REFERENCES public.role(id);


--
-- TOC entry 2988 (class 0 OID 0)
-- Dependencies: 6
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


-- Completed on 2020-10-22 00:08:23

--
-- PostgreSQL database dump complete
--

