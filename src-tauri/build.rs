use std::process::Command;
use std::env;
use std::path::Path;

fn main() {
    println!("cargo:rerun-if-changed=tauri.conf.json");

    if cfg!(target_os = "windows") {
        let _ = Command::new("taskkill")
            .args(&["/F", "/IM", "FpRail.Presentation*"]) // Обратите внимание на имя процесса, если оно изменилось
            .output();
    }

    let manifest_dir = env::var("CARGO_MANIFEST_DIR").unwrap();
    let root_dir = Path::new(&manifest_dir).parent().unwrap();
    
    // Путь к папке конкретно вашего Presentation-слоя
    let backend_dir = root_dir.join("src-backend");
    let presentation_project = backend_dir.join("FpRail.Presentation").join("FpRail.Presentation.csproj"); 
    
    let target_binaries_dir = Path::new(&manifest_dir).join("binaries");
    let temp_publish_dir = Path::new(&manifest_dir).join("target").join("backend_publish");

    std::fs::create_dir_all(&target_binaries_dir).unwrap();
    std::fs::create_dir_all(&temp_publish_dir).unwrap();

    // 3. Собираем конкретный .csproj, а не все папки подряд
    let status = Command::new("dotnet")
        .current_dir(&backend_dir) // Выполняем в контексте бэкенда
        .args(&[
            "publish",
            presentation_project.to_str().unwrap(), // Передаем путь к конкретному проекту!
            "-c", "Release",
            "-r", "win-x64",
            "--self-contained", "true",
            "/p:PublishSingleFile=true",
            "/p:PublishReadyToRun=true",
            "/p:PublishSingleFileSplit=false", // Дополнительно склеиваем PDB и конфигурации по возможности
            "-o", temp_publish_dir.to_str().unwrap(),
        ])
        .status()
        .expect("Не удалось запустить сборку .NET бэкенда");

    if !status.success() {
        panic!("Ошибка компиляции ASP.NET Core API!");
    }

    // 4. Копируем получившийся монолитный исполняемый файл
    let target_triple = env::var("TARGET").unwrap(); 
    
    // Имя исходного файла теперь будет соответствовать вашему слою Presentation
    let source_exe = temp_publish_dir.join("FpRail.Presentation.exe");
    let dest_exe = target_binaries_dir.join(format!("FpRail.WebApi-{}.exe", target_triple));

    std::fs::copy(&source_exe, &dest_exe).expect("Не удалось скопировать бинарник в папку Tauri");

    // Копируем appsettings
    let appsettings = temp_publish_dir.join("appsettings.json");
    let dest_settings = target_binaries_dir.join("appsettings.json");
    if appsettings.exists() {
        let _ = std::fs::copy(&appsettings, &dest_settings);
    }

    let _ = std::fs::remove_dir_all(&temp_publish_dir);

    tauri_build::try_build(tauri_build::Attributes::new())
        .expect("failed to run tauri-build");
}
