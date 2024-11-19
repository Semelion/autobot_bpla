#include <EEPROM.h>
#include "painlessMesh.h"
#include <GSON.h> // Подключаем библиотеку gson

Scheduler userScheduler; // Планировщик задач
painlessMesh mesh;

void setup() {
  EEPROM.begin(1024);
  
  mesh.setDebugMsgTypes(ERROR | STARTUP);
  
  int addres = 1;
  int MESH_PORT;
  EEPROM.get(addres, MESH_PORT);
  addres += 4;

  char MESH_PREFIX[40];
  EEPROM.get(addres, MESH_PREFIX);
  addres += 40;

  char MESH_PASSWORD[40];
  EEPROM.get(addres, MESH_PASSWORD);
  addres += 40;

  mesh.init(MESH_PREFIX, MESH_PASSWORD, &userScheduler, MESH_PORT);
  mesh.onReceive(&receivedCallback);
  mesh.onNewConnection(&newConnectionCallback);
  mesh.onChangedConnections(&changedConnectionCallback);
  mesh.onNodeTimeAdjusted(&nodeTimeAdjustedCallback);

  Serial.begin(115200);
  while (!Serial);
}

void loop() {
  mesh.update();
  // static long long time = 0;
  // if(abs(millis() - time) >= 500){
  //   gson::Parser parser;
  //   parser.parse("{\"command\": 1,\"ID\": 0,\"message\": \"Hello world5\"}");
  //   sendMessage(parser);
  //   Serial.println("send");
  //   time = millis();
  // }

  if (Serial.available()) {
    String inputString = Serial.readStringUntil('\n');
    gson::Parser parser;
  
    //c = 0 - setup
    // {"command": 0,"ID": 0,"ssid": "test","pwd": "1234","ip_port": 5555}
    //c = 1 - message to id
    // {"command": 1,"ID": 0,"message": "Hello world"}
    //c = 2 - print setup
    // {"command": 2}


    // Парсинг JSON
    if (!parser.parse(inputString.c_str())) {
      Serial.println("Invalid JSON received");
      return;
    }

    if (parser.has("command")) {
      int command = parser.get("command").toInt();
      switch (command) {
        case 0: // Настройка
          set_settings(parser);
          break;
        case 1: // Отправка сообщения
          sendMessage(parser);
          break;
        case 2: // Вывод конфигурации
          print_setup();
          break;
        default:
          Serial.println("Exception: wrong command");
      }
    }
  }
}

void set_settings(gson::Parser& parser) {
  int addres = 0;

  if (parser.has("ID")) {
    EEPROM.write(addres, parser.get("ID").toInt());
  }
  addres += 1; // 1 байт

  if (parser.has("ip_port")) {
    int ip_port = parser.get("ip_port").toInt();
    EEPROM.put(addres, ip_port);
  }
  addres += 4; // 4 байта

  if (parser.has("ssid")) {
    String ssid = parser.get("ssid").toString();
    if (sizeof(ssid) > 40) {
      Serial.println("Error: SSID is too large");
    } else {
      EEPROM.put(addres, ssid);
    }
  }
  addres += 40; // 40 байт

  if (parser.has("pwd")) {
    String pwd = parser.get("pwd").toString();
    if (sizeof(pwd) > 40) {
      Serial.println("Error: password is too large");
    } else {
      EEPROM.put(addres, pwd);
    }
  }
  addres += 40; // 40 байт

  EEPROM.commit();
  ESP.restart();
}

void print_setup() {
  int addres = 0;

  Serial.print("ID: ");
  Serial.println(EEPROM.read(addres));
  addres += 1;

  Serial.print("ip_port: ");
  int ip_port;
  EEPROM.get(addres, ip_port);
  Serial.println(ip_port);
  addres += 4;

  Serial.print("ssid: ");
  String ssid;
  EEPROM.get(addres, ssid);
  Serial.println(ssid);
  addres += 40;

  Serial.print("pwd: ");
  String pwd;
  EEPROM.get(addres, pwd);
  Serial.println(pwd);
  addres += 40;
}

void sendMessage(gson::Parser& parser) {
  if (parser.has("ID")) {
    String msg = "{\"ID\":";
    msg += parser.get("ID").toInt();

    if (parser.has("message")) {
      msg += ",\"message\":\"";
      msg += parser.get("message").toString();
      msg += "\"}";
      
      mesh.sendBroadcast(msg);
    } else {
      Serial.println("Exception: no message");
    }
  } else {
    Serial.println("Exception: no ID");
  }
}

void receivedCallback(uint32_t from, String &msg) {
  gson::Parser parser;
  if (!parser.parse(msg.c_str())) {
    Serial.println("Invalid JSON received from system");
    return;
  }
  if (parser.has("ID")) 
    if(EEPROM.read(0) == parser.get("ID").toInt())
      if(parser.has("message"))
        Serial.println(parser.get("message"));
}

void newConnectionCallback(uint32_t nodeId) {
  Serial.printf("--> New Connection, nodeId = %u\n", nodeId);
}

void changedConnectionCallback() {
  Serial.printf("Changed connections\n");
}

void nodeTimeAdjustedCallback(int32_t offset) {
  Serial.printf("Adjusted time %u. Offset = %d\n", mesh.getNodeTime(), offset);
}