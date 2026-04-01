# WebShop

一个前后端分离的电商示例项目，包含：

- `WebApi`：ASP.NET Core 8 后端（JWT 鉴权、Swagger、商品/订单/用户管理、图片上传）
- `admin-web`：Vue 3 + TypeScript + Element Plus 管理后台
- `shop-mp`：微信原生小程序用户端（商品浏览、下单、订单、我的）

## 项目结构

```text
WebShop
├─ WebApi/                 # API 层（控制器、启动配置、Swagger）
├─ Application/            # 应用层接口与业务抽象
├─ Domain/                 # 领域实体
├─ Infrastructure/         # 数据访问、服务实现、迁移、种子数据
├─ admin-web/              # 管理后台（Vue3 + TS + Vite）
├─ shop-mp/                # 微信原生小程序
└─ WebShop.sln
```

## 技术栈

### Backend

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core（Migrations）
- JWT 鉴权
- Swagger（支持 Bearer Token）

### Admin Web

- Vue 3 + TypeScript + Vite
- Pinia
- Vue Router
- Element Plus
- Axios

### Mini Program

- 微信原生小程序（TypeScript）
- `wx.request` 封装 HTTP

## 功能概览

- 管理端登录（JWT）
- 仪表盘摘要统计
- 商品管理（增删改查 + 图片上传）
- 订单管理（状态流转）
- 用户管理（启用/禁用、新增）
- 小程序端：登录、商品列表、商品详情、下单、订单、我的

## 本地运行

## 1) 启动后端 WebApi

推荐在仓库根目录执行：

```bash
dotnet run --project WebApi
```

默认地址（见 `WebApi/Properties/launchSettings.json`）：

- `http://localhost:5000`
- `https://localhost:7191`

Swagger：

- `http://localhost:5000/swagger`

## 2) 启动管理后台 admin-web

```bash
cd admin-web
npm install
npm run dev
```

开发地址：

- `http://localhost:5173`

代理目标配置（见 `admin-web/.env.development`）：

- `VITE_API_TARGET=http://localhost:5000`

> 项目还提供 `npm run dev:wait-api`，会先等待 `127.0.0.1:5000` 可连接，再启动 Vite，适合联调时避免前端先起导致的 API 连接报错。

## 3) 运行微信小程序 shop-mp

1. 用微信开发者工具打开 `shop-mp/`
2. 确认后端已启动（`http://localhost:5000`）
3. API 基础地址在 `shop-mp/utils/config.ts`：

```ts
export const API_BASE_URL = 'http://localhost:5000/api'
```

## 默认测试账号

管理后台登录页内置默认值：

- 用户名：`admin`
- 密码：`Admin123!`

（如需修改，请调整种子数据或登录页默认表单值）

## 常用开发命令

### .NET

```bash
dotnet restore
dotnet build WebShop.sln
dotnet test
```

### admin-web

```bash
cd admin-web
npm install
npm run dev
npm run build
```

## 调试建议

- VS Code 已包含联调配置：`Fullstack (WebApi + Admin-Web)`
- 若前端提示 API 连接失败，优先检查：
  - WebApi 是否已启动在 `5000`
  - `admin-web/.env.development` 的 `VITE_API_TARGET` 是否一致
  - 修改 `.env` 后是否重启了 Vite

## 说明

- 本仓库为学习/演示项目，可按需拆分为独立后端服务与前端应用。
- 首次提交已推送到 `main` 分支，可在此基础上继续迭代（如 CI/CD、容器化、权限细分、自动化测试等）。
