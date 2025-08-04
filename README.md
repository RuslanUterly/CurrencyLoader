<h1>Приложение CurrencyLoader (.NET 8 + PostgreSQL)</h1>

В качестве БД используется PostgreSQL. 
Так как в основном проекте нет EF Core и Dapper было решено использовать "Чистый SQL" и паттерны UnitOfWork и Repository

<h3>Что можно улучшить?</h3>

<li>В будущем разделить приложения на слои, внедря либо луковую, либо чистую архитектуру</li>
<li>Использовать UserSecrets, чтобы не показывать реальные строки подключения к БД</li>

<h3>Запуск и отладка Docker для Rider</h3>
Сначала выполнить команду: docker compose down -v
Следующий шаг это перейти в DockerFile и раскоментировать нужную строчку
<img width="716" height="249" alt="image" src="https://github.com/user-attachments/assets/7fa35453-6843-4aa4-a152-8e05b8bc8851" />
Затем необходимо перейти в файл compose.yaml и выполнить его после чего появится контейнер и возможность выбрать в качестве запуска и дебага docker-compose

<h3>Запуск и отладка Docker для VisalStudio</h3>
Сначала выполнить команду: docker compose down -v
Следующий шаг это перейти в DockerFile и раскоментировать нужную строчку
<img width="716" height="249" alt="image" src="https://github.com/user-attachments/assets/7fa35453-6843-4aa4-a152-8e05b8bc8851" />
Затем необходимо дебаге запустить docker-compose

<h3>Структура БД</h3>
<img width="612" height="291" alt="image" src="https://github.com/user-attachments/assets/7659b9a7-20b9-47dc-adf9-fec3673dbeeb" />
