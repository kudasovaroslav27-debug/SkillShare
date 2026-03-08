# SkillShare — Система управления обучением (LMS)

Этот проект представляет собой современную платформу для онлайн-образования на **ASP.NET Web API**. Система позволяет создавать иерархические курсы, управлять уроками, проводить тестирование и рассчитывать успеваемость.

## Особенности

• **Проект на .NET 9** — использование актуальных возможностей платформы.
• **Управление обучением** — создание курсов, уроков и вопросов с поддержкой дерева курсов.
• **Автоматизированное тестирование** — проверка ответов в реальном времени.
• **Отслеживание прогресса** — расчет среднего балла и успеваемости по курсам.
• **Clean Architecture & DDD** — разделение на слои и богатые доменные модели.
• **Event-Driven** — асинхронное взаимодействие через RabbitMQ.
• **Паттерны проектирования** — CQRS (MediatR), Репозиторий, UnitOfWork, Фасад.

## Технологии

• **ASP.NET Web API** — основной фреймворк.
• **Entity Framework Core** — работа с БД (Code First).
• **PostgreSQL** — основное хранилище.
• **MediatR** — реализация паттерна CQRS.
• **RabbitMQ** — брокер сообщений.
• **Mapster** — маппинг объектов.
• **Redis** — кэширование данных.
• **JWT Bearer** — аутентификация и авторизация.
• **FluentValidation** — валидация моделей.
• **Docker** — контейнеризация.
• **Prometheus & Grafana** — мониторинг и метрики.
## Тестирование и качество
• **xUnit & Moq** — модульное тестирование бизнес-логики.

## Установка и запуск
1. **Клонируйте репозиторий** на свой компьютер.
2. **Настройте секреты** (User Secrets). Пример конфигурации:
1. 
  {
  "ConnectionStrings:PostgresSQL": "Server=localhost;Port=5432;Database=SkillShareDb;Username=postgres;Password=postgres",
  "RedisSettings:Url": "localhost:6379",
  "RabbitMqSettings:Host": "localhost",
  "JwtSettings:JwtKey": "your_very_long_secret_key_here_32_chars_min",
  "AdminSettings:Login": "admin",
  "AdminSettings:Password": "12345",
  "ElasticConfiguration:Uri": "http://localhost:9200"
}
  Перейдите в терминале в папку deploy и выполните команду: docker-compose up -d
3. Запуск API:
  Для запуска контейнера API добавьте в папку deploy/.env конфиги (используйте названия сервисов вместо localhost). Пример в файле .env.template.
4. Документация:
  После запуска Swagger доступен по адресу:
  •  Локально: https://localhost:7281/swagger/index.html
  •  В Docker: https://localhost:5001/swagger/index.html

 Пример использования
1. Авторизация:
  Используйте метод POST /auth/login с кредами админа из ваших настроек.
     {
      "login": "admin",
      "password": "12345"
     }
2. Получение токена:
  Скопируйте AccessToken из ответа и вставьте его в окно Authorize в Swagger (формат: Bearer {token}).
3. Прохождение урока:
  Метод POST /lessons/pass принимает массив ваших ответов на вопросы. Система автоматически рассчитает балл и обновит ваш прогресс по курсу.
Developed with ❤️ using .NET 9