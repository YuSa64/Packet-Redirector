# Packet Redirector

**Packet Redirector**는 특정 IP와 포트로 수신된 패킷을 실시간으로 다른 IP와 포트로 전달(포워딩)해주는 간단하고 가벼운 GUI 기반 프로그램입니다.  
UDP / TCP 모두 지원하며, 실시간 패킷 확인 기능 및 다국어 지원을 제공합니다.

---

## 주요 기능

-  UDP / TCP 패킷 리다이렉팅 지원
-  수신 및 송신 포트/프로토콜 지정 가능
-  초당 수신 패킷 수 실시간 표시
-  최근 수신된 패킷 메시지 확인 가능
-  수정 가능한 다국어 UI (예: English / 한국어)
-  설정 자동 저장 및 복원 (`config.json`)

---

## 사용 방법

### 1. 실행
> `PacketRedirector.exe` 실행  
> (설치 없이 바로 실행 가능)

### 2. 언어 선택
> 상단의 `Language` 드롭다운에서 언어 선택

### 3. 리다이렉팅 설정

| 항목 | 설명 |
|------|------|
| Receive Protocol | 수신 프로토콜 (UDP 또는 TCP) |
| Receive IP / Port | 수신 IP 주소와 포트 (기본: 127.0.0.1 / 39539) |
| Send Protocol | 송신 프로토콜 (UDP 또는 TCP) |
| Send IP / Port | 전송할 대상 IP 주소와 포트 |

> 설정은 자동으로 저장되며, 다음 실행 시 복원됩니다.

### 4. 실행/정지

- `Start` 버튼 클릭 → 포워딩 시작
- `Stop` 버튼 클릭 → 중단

### 5. 패킷 확인

- 하단 `Last Packet Message` 패널을 열면  
  가장 최근 수신된 메시지를 확인할 수 있습니다.

---

## 다국어 설정

`Locales/` 폴더에 JSON 형식으로 언어별 UI 번역을 관리합니다.  
기본적으로 `en.json`, `ko.json`이 제공되며, 새로운 언어 파일을 추가하면 자동으로 인식됩니다.

예시 (`Locales/en.json`):

```json
{
  "lang": "English",
  "text": {
    "title": "Packet Redirector",
    "start": "Start",
    "stop": "Stop",
    ...
  }
}
