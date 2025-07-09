# Catalogo-Backend-
Este proyecto consiste en el desarrollo de un sistema web que gestiona usuarios, roles, permisos y un catálogo de productos. Está diseñado con una arquitectura modular y escalable, permitiendo añadir nuevas funcionalidades conforme evolucione el sistema.

🎯 Objetivo General
Crear una plataforma segura y administrable que permita a usuarios autenticarse, gestionar su cuenta y explorar productos, mientras que usuarios con permisos de administrador puedan gestionar la configuración del sistema, usuarios y catálogos.

Tecnologías

Backend: .Net
Base de datos: PostgreSQL / MySQL
ORM: Entity Framework Core (EF Core)
Control de versiones: Git con flujo GitFlow


Configuración inicial
**Clonar el repositorio correspondiente**:
    git clone <url-bsckend-repo>
    cd backend-repo

Branch principal: main
Branch de desarrollo: develop
Prefixes por defecto: feature/, release/, hotfix/, support/

**Creación de ramas de trabajo:**
Features: para nueva funcionalidad o historias de usuario:
git flow feature start nombre-de-la-feature

Release: para preparar una versión estable antes de producción:
git flow release start v1.0.0

Hotfix: para correcciones urgentes en producción:
git flow hotfix start hotfix-descripcion

Finalizar y publicar ramas:
Terminar feature:
git flow feature finish nombre-de-la-feature
git push origin develop

Terminar release:
git flow release finish v1.0.0
git push origin main develop --tags

Terminar hotfix:
git flow hotfix finish hotfix-descripcion
git push origin main develop --tags

Políticas de Pull Request / Merge Request:
Siempre desde feature/* a develop.
Revisiones por al menos un revisor antes de merge.
Mensajes claros describiendo la historia o fix implementado.

Etiquetas y versiones:
Usar semantic versioning (MAJOR.MINOR.PATCH).
Las etiquetas (git tag) se crean automáticamente al finalizar releases o hotfixes.
