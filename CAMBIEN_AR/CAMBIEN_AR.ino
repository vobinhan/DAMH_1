  #define sensor 2
int cb;
void setup()
{
  pinMode(sensor,INPUT);
  Serial.begin(9600);
}
void loop()
{
  cb=digitalRead(sensor);
  Serial.print("");
  Serial.println(cb);
  delay(500);
}
