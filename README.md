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

## Docker 快速启动

仓库已提供：

- `WebApi/Dockerfile`
- `admin-web/Dockerfile`
- `admin-web/nginx.conf`
- `docker-compose.yml`（含 SQL Server / WebApi / admin-web）

在仓库根目录执行：

```bash
docker compose up -d --build
```

启动后访问：

- 管理端：`http://localhost:5173`
- 后端 Swagger：`http://localhost:5000/swagger`

说明：

- `webapi` 启动时会自动执行 EF Core `Migrate()`，并按配置执行种子数据。
- SQL Server 默认密码来自 `SA_PASSWORD`，未设置时使用 compose 中的默认值（建议在本地用 `.env` 覆盖）。
- 上传图片目录已挂载卷：`/app/wwwroot/uploads`。

## CI/CD（GitHub Actions）

已提供两个工作流：

- `CI`：`.github/workflows/ci.yml`
  - 后端：`dotnet restore/build/test`
  - 前端：`admin-web npm ci + npm run build`
- `Docker Publish`：`.github/workflows/docker-publish.yml`
  - 在 `main` push 或手动触发时，构建并推送镜像到 GHCR：
    - `ghcr.io/<owner>/webshop-webapi`
    - `ghcr.io/<owner>/webshop-admin-web`

## 调试建议

- VS Code 已包含联调配置：`Fullstack (WebApi + Admin-Web)`
- 若前端提示 API 连接失败，优先检查：
  - WebApi 是否已启动在 `5000`
  - `admin-web/.env.development` 的 `VITE_API_TARGET` 是否一致
  - 修改 `.env` 后是否重启了 Vite

#