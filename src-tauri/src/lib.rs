use tauri_plugin_shell::ShellExt;
use std::time::Duration;
use tokio::time::sleep;

#[tauri::command]
async fn create_tram_point(feature: serde_json::Value) -> Result<serde_json::Value, String> {
    let url = "http://localhost:5042/api/v1/tram-points";
    let client = reqwest::Client::new();
    
    let mut attempts = 5;
    let mut last_error = String::new();

    while attempts > 0 {
        match client.post(url).json(&feature).send().await {
            Ok(response) => {
                if response.status().is_success() {
                    let json_body = response.json::<serde_json::Value>().await
                        .map_err(|e| format!("Ошибка парсинга JSON от .NET: {}", e))?;
                    return Ok(json_body);
                } else {
                    return Err(format!(".NET бэкенд вернул код ошибки: {}", response.status()));
                }
            }
            Err(e) => {
                last_error = e.to_string();
                println!("Бэкенд еще не готов, пробуем снова... Осталось попыток: {}", attempts - 1);
                sleep(Duration::from_millis(300)).await;
                attempts -= 1;
            }
        }
    }

    Err(format!("Не удалось связаться с .NET Sidecar после нескольких попыток. Ошибка: {}", last_error))
}

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_shell::init())
        .plugin(tauri_plugin_http::init())
        .invoke_handler(tauri::generate_handler![create_tram_point])
        .setup(|app| {
            // 1. Получаем команду Sidecar (переменная mut не нужна)
            let sidecar_command = app.shell().sidecar("FpRail.WebApi").unwrap();

            // 2. Спавним асинхронный процесс
            let (mut rx, _child) = sidecar_command
                .spawn()
                .expect("Failed to spawn .NET Sidecar");

            // 3. Запускаем фоновую задачу Rust (Task) для чтения логов
            tauri::async_runtime::spawn(async move {
                while let Some(event) = rx.recv().await {
                    match event {
                        tauri_plugin_shell::process::CommandEvent::Stdout(line) => {
                            // Пишем в консоль терминала то, что прилетело из Console.WriteLine бэкенда
                            let log_string = String::from_utf8_lossy(&line);
                            print!("[.NET-Backend]: {}", log_string);
                        }
                        tauri_plugin_shell::process::CommandEvent::Stderr(line) => {
                            let err_string = String::from_utf8_lossy(&line);
                            eprint!("[.NET-Error]: {}", err_string);
                        }
                        tauri_plugin_shell::process::CommandEvent::Terminated(payload) => {
                            println!("[.NET-Backend]: Процесс завершился с кодом {:?}", payload.code);
                        }
                        _ => {}
                    }
                }
            });

            Ok(())
        })
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
