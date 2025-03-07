import { View, Text } from 'react-native'
import React from 'react'
import { NativeStackNavigationProp } from '@react-navigation/native-stack'
import { RootStackParamsList } from '../../routes'


const Signup = ({ navigation }: SignupScreenProps) => {
  return (
    <View>
      <Text>Signup</Text>
    </View>
  )
}

export default Signup

interface SignupScreenProps {
  navigation: NativeStackNavigationProp<RootStackParamsList, 'Signup'>
}