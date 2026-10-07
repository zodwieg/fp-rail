use tauri_plugin_shell::ShellExt;

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
    // 1. Обязательно инициализируем плагин shell
        .plugin(tauri_plugin_shell::init())
        .plugin(tauri_plugin_http::init())
        .setup(|app| {
            // 2. Указываем относительный путь к Sidecar (без трипла и .exe)
            let sidecar_command = app.shell().sidecar("FpRail.WebApi").unwrap();

            // 3. Запускаем .NET-процесс параллельно в фоне и обрабатываем Result
            let _child = sidecar_command
                .spawn()
                .expect("Failed to spawn .NET Sidecar");

            Ok(())
        })
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
